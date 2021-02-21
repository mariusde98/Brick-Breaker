using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGeneratorScript : MonoBehaviour
{
    [Header("Setting for each Spherelayer")]
    public SphereSettings[] sphereSettings;

    // The prefab of the spherical hull draped around the blocks
    // (this will be scaled up, so its original form should have size 1)
    public GameObject hullPrefab;

    // The prefab used for the two collision planes, defining the allowed play area
    // (they will be scaled up, so its original form should have size 1)
    public GameObject collisionPlanePrefab;

    [Header("Gizmos Settings")]
    public bool gizmosDrawClamps = true;
    public bool gizmosDrawBlocks = true;

    //Private Variables

    //Matrix of the blockGenerator Transform
    private Matrix4x4 matrix;

    //For Fibonacci sphere
    private float goldenRatio;
    private float angleIncrement;

    // Size of the block prefab, used for drawing the Gizmos & adjusting the collision planes
    private Vector3 blockSize;

    // This event is sent when a block is destroyed by the ball
    public BrickDestroyedEvent onBlockDestroyed;

    private GameObject hull;
    private GameObject topCollisionPlane;
    private GameObject bottomCollisionPlane;

    //Representing the count of witch sphere layers should get spawned from inner to outer
    // 0 is most inner sphere layer
    private int initiatedSphereLayers;
    private bool reachedLastLayer = false;

    // Start is called before the first frame update
    void Start()
    {
        // generateSphere();
    }

    public void UpdateSphereLayers(int newSphereLayer)
    {
        if (newSphereLayer > initiatedSphereLayers)
        {
            if (newSphereLayer < sphereSettings.Length)
            {
                InstantiatePrefabs(initiatedSphereLayers + 1, newSphereLayer + 1);
                initiatedSphereLayers = newSphereLayer;
            }
            else
            {
                reachedLastLayer = true;
                Debug.LogWarning("Reached last Sphere layer, cant genarate a new one");
            }
        }
    }

    //Event is called by the LevelEvent 
    public void OnLevelEvent(int levelEvent)
    {
        if (levelEvent == LevelEvent.LEVEL_START)
        {
            //Generatting the sphere when the level starts
            GenerateSphere();
        }
    }

    //Public Method to generate the sphere
    [ContextMenu("generate Sphere")]
    public void GenerateSphere()
    {
        DeleteSphere();
        GenerateStartingValues();
        InstantiatePrefabs(0, 1);
    }

    //Public Method to delete all generated Prefabs (transform.child's)
    [ContextMenu("delete Sphere")]
    public void DeleteSphere()
    {
        foreach (Transform child in transform)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }

        onBlockDestroyed.RemoveAllListeners();
    }

    //Generating some values for calculation and for the gizmos,
    //also generating the transform matrix
    private void GenerateStartingValues()
    {
        //These values will be the same 
        if (goldenRatio == 0f)
        {
            //For Calculation (fibonacci Sphere)
            goldenRatio = (1 + Mathf.Sqrt(5f)) / 2f;
            angleIncrement = Mathf.PI * 2 * goldenRatio;
        }

        //These values are just for the gizmos, it will only be generated once or it will be updated if running in Editor
        if (Application.isEditor || blockSize == Vector3.zero)
        {
            SphereSettings currentSettings = sphereSettings[0];

            //Setting the blockSize to the bound of the block prefab
            if (currentSettings.defaultPrefabs.Length > 0 && currentSettings.defaultPrefabs[0])
            {
                blockSize = currentSettings.defaultPrefabs[0].GetComponent<Renderer>().bounds.size;
            }

            if (blockSize == Vector3.zero)
            {
                blockSize = Vector3.one;
            }
        }

        //Generating offset Matrix
        matrix = Matrix4x4.Translate(this.transform.position) * Matrix4x4.Rotate(this.transform.rotation) * Matrix4x4.Scale(this.transform.localScale);
    }


    //Private Method for instantiating the prefabs of each sphereLayer
    //startlayer is included and endLayer is excluded
    private void InstantiatePrefabs(int startLayer, int endLayer)
    {
        // Keep track of the largest layer's radius & clamp values,
        // as the hull & collision planes need to be wrapped around that layer
        float largestRadius = 0;
        float highestClampY = float.MinValue;
        float lowestClampY = float.MaxValue;

        float centerY = this.transform.position.y;

        for (int sphereIndex = startLayer; sphereIndex < endLayer; sphereIndex++)
        {
            SphereSettings currentSettings = sphereSettings[sphereIndex];

            // Check radius of this layer
            if (currentSettings.radius > largestRadius)
            {
                largestRadius = currentSettings.radius;
            }

            // Check collision plane adjustments for this layer
            float bottomY = (centerY - currentSettings.radius) + (currentSettings.clampY.x * currentSettings.radius * 2);
            float topY = (centerY - currentSettings.radius) + (currentSettings.clampY.y * currentSettings.radius * 2);

            if (bottomY < lowestClampY)
            {
                lowestClampY = bottomY;
            }

            if (topY > highestClampY)
            {
                highestClampY = topY;
            }

            for (int x = 0; x < currentSettings.blockCount; x++)
            {
                //Getting the points position on the current Sphere Layer
                Vector3 v = GetPointPosition(x, sphereIndex);
                if (ClampVector(v, sphereIndex))
                {
                    //Setting the randomBlock to a default block with all the same chance
                    GameObject randomBlock = currentSettings.defaultPrefabs[Random.Range(0, currentSettings.defaultPrefabs.Length)];

                    //Checking if one special chance had the chance to spawn. If so the randombBlock will get set to that special block
                    for (int specialBlockCount = 0; specialBlockCount < currentSettings.specialPrefabs.Length; specialBlockCount++)
                    {
                        if (Random.Range(0f, 1f) <= currentSettings.specialChance[specialBlockCount])
                        {
                            randomBlock = currentSettings.specialPrefabs[specialBlockCount];
                            break;
                        }
                    }

                    //Instantiateing the block and rotating at towards the generator 
                    InstantiateBlock(randomBlock, ApplyMatrix(v, sphereIndex));
                }
            }
        }

        // Instantiate the outer hull & collision planes,
        // then scale them up to be as large as the largest layer
        Vector3 desiredScale = new Vector3(largestRadius, largestRadius, largestRadius);

        float blockPadding = blockSize.y / 2;

        if (!hull)
        {
            hull = Instantiate(hullPrefab, this.transform.position, this.transform.rotation, this.transform);
        }
        hull.transform.localScale = desiredScale;

        if (!topCollisionPlane)
        {
            // Rotate the top plane by 180 degrees, otherwise the collisions will be weird
            topCollisionPlane = Instantiate(
                collisionPlanePrefab,
                this.transform.position,
                Quaternion.Euler(0, 0, 180),
                this.transform
            );
        }
        topCollisionPlane.transform.localScale = desiredScale;
        topCollisionPlane.transform.position = new Vector3(
            topCollisionPlane.transform.position.x,
            highestClampY + blockPadding, // Prevent blocks from partially sticking inside the collision plane
            topCollisionPlane.transform.position.z
        );

        if (!bottomCollisionPlane)
        {
            bottomCollisionPlane = Instantiate(
                collisionPlanePrefab,
                this.transform.position,
                Quaternion.identity,
                this.transform);
        }
        bottomCollisionPlane.transform.localScale = desiredScale;
        bottomCollisionPlane.transform.position = new Vector3(
            bottomCollisionPlane.transform.position.x,
            lowestClampY - blockPadding, // Prevent blocks from partially sticking inside the collision plane
            bottomCollisionPlane.transform.position.z
        );
    }

    private void InstantiateBlock(GameObject prefab, Vector3 position)
    {
        GameObject block = Instantiate(prefab, position, Quaternion.identity, this.transform) as GameObject;
        block.transform.LookAt(this.transform, Vector3.up);

        // When the block is destroyed, notify the generator about it
        BrickController controller = block.GetComponent<BrickController>();
        if (controller != null)
        {
            // Forward the event to the generator's own "OnBlockDestroyed" event
            // (the GameManager will listen to it)
            controller.onDestroyed.AddListener((effects, ballController) => onBlockDestroyed.Invoke(effects, ballController));
        }
    }

    //return will be between 1 and -1 coordinates
    private Vector3 GetPointPosition(int index, int sphereIndex)
    {
        SphereSettings currentSettings = sphereSettings[sphereIndex];
        //Genearting Points on the fibonacci Sphere

        float t = index / (float)currentSettings.blockCount;
        float angle1 = Mathf.Acos(1 - 2 * t);
        float angle2 = angleIncrement * index;

        float x = Mathf.Sin(angle1) * Mathf.Cos(angle2);
        float y = Mathf.Sin(angle1) * Mathf.Sin(angle2);
        float z = Mathf.Cos(angle1);

        Vector3 v = new Vector3(x, y, z);

        return v;
    }


    //Is the given vector inside the clamp range
    private bool ClampVector(Vector3 input, int sphereIndex)
    {
        SphereSettings currentSettings = sphereSettings[sphereIndex];
        //Clamping the values in x,y,z directions
        if (currentSettings.clampX.x > 0 && input.x <= (currentSettings.clampX.x * 2 - 1))
        {
            return false;
        }
        if (currentSettings.clampX.y < 1 && input.x >= (currentSettings.clampX.y * 2 - 1))
        {
            return false;
        }

        if (currentSettings.clampY.x > 0 && input.y <= (currentSettings.clampY.x * 2 - 1))
        {
            return false;
        }
        if (currentSettings.clampY.y < 1 && input.y >= (currentSettings.clampY.y * 2 - 1))
        {
            return false;
        }

        if (currentSettings.clampZ.x > 0 && input.z <= (currentSettings.clampZ.x * 2 - 1))
        {
            return false;
        }
        if (currentSettings.clampZ.y < 1 && input.z >= (currentSettings.clampZ.y * 2 - 1))
        {
            return false;
        }


        return true;
    }

    //Applying the radius of the current sphereLayer and the transform matrix of this object
    private Vector3 ApplyMatrix(Vector3 input, int sphereIndex)
    {
        input *= sphereSettings[sphereIndex].radius;
        return matrix.MultiplyPoint3x4(input);
    }


    private void OnDrawGizmos()
    {
        if (transform.hasChanged)
        {
            OnValidate();
        }

        //Drawing the clamps
        if (gizmosDrawClamps)
        {
            for (int sphereIndex = 0; sphereIndex < sphereSettings.Length; sphereIndex++)
            {
                SphereSettings currentSettings = sphereSettings[sphereIndex];
                if (currentSettings)
                {
                    Gizmos.color = Color.red;
                    if (currentSettings.clampX.x > 0)
                    {
                        Gizmos.DrawWireCube(ApplyMatrix(new Vector3(currentSettings.clampX.x * 2 - 1, 0, 0), sphereIndex), (ApplyMatrix(new Vector3(0, 2, 2), sphereIndex)));
                    }
                    if (currentSettings.clampX.y < 1)
                    {
                        Gizmos.DrawWireCube(ApplyMatrix(new Vector3(currentSettings.clampX.y * 2 - 1, 0, 0), sphereIndex), (ApplyMatrix(new Vector3(0, 2, 2), sphereIndex)));
                    }

                    Gizmos.color = Color.green;
                    if (currentSettings.clampY.x > 0)
                    {
                        Gizmos.DrawWireCube(ApplyMatrix(new Vector3(0, currentSettings.clampY.x * 2 - 1, 0), sphereIndex), (ApplyMatrix(new Vector3(2, 0, 2), sphereIndex)));
                    }
                    if (currentSettings.clampY.y < 1)
                    {
                        Gizmos.DrawWireCube(ApplyMatrix(new Vector3(0, currentSettings.clampY.y * 2 - 1, 0), sphereIndex), (ApplyMatrix(new Vector3(2, 0, 2), sphereIndex)));
                    }

                    Gizmos.color = Color.blue;
                    if (currentSettings.clampZ.x > 0)
                    {
                        Gizmos.DrawWireCube(ApplyMatrix(new Vector3(0, 0, currentSettings.clampZ.x * 2 - 1), sphereIndex), (ApplyMatrix(new Vector3(2, 2, 0), sphereIndex)));
                    }
                    if (currentSettings.clampZ.y < 1)
                    {
                        Gizmos.DrawWireCube(ApplyMatrix(new Vector3(0, 0, currentSettings.clampZ.y * 2 - 1), sphereIndex), (ApplyMatrix(new Vector3(2, 2, 0), sphereIndex)));
                    }
                }
            }
        }



        //Generating gizmos at the position of the blocks that will be placed at runtime
        if (gizmosDrawBlocks)
        {
            Gizmos.color = Color.black;
            for (int sphereIndex = 0; sphereIndex < sphereSettings.Length; sphereIndex++)
            {
                SphereSettings currentSettings = sphereSettings[sphereIndex];
                for (int x = 0; x < currentSettings.blockCount; x++)
                {
                    Vector3 v = GetPointPosition(x, sphereIndex);
                    if (ClampVector(v, sphereIndex))
                    {
                        Gizmos.DrawWireCube(ApplyMatrix(v, sphereIndex), blockSize);
                    }
                }
            }
        }
    }

    public bool IsReachedLastLayer()
    {
        return reachedLastLayer;
    }


    //For Editor Workflow, regenerating the values if they change, so the gizmos will change
    [ContextMenu("Validate")]
    private void OnValidate()
    {
        if (sphereSettings[0])
        {
            GenerateStartingValues();
        }

        //Setting the array length of specialChance as the same as specialPrefabs if its not
        for (int sphereIndex = 0; sphereIndex < sphereSettings.Length; sphereIndex++)
        {
            SphereSettings currentSettings = sphereSettings[sphereIndex];
            if (currentSettings)
            {
                if (currentSettings.specialPrefabs.Length != currentSettings.specialChance.Length)
                {
                    currentSettings.specialChance = new float[currentSettings.specialPrefabs.Length];
                }
            }
        }
    }




}
