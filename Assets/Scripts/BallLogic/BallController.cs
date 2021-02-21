using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallsHolderSingleton
{
    private BallsHolderSingleton() { }
    private static readonly object padlock = new object();
    private static BallsHolderSingleton instance = null;

    // Accessor for the single instance of the BallsHolder
    // (used by other scripts to grab all current balls in the scene)
    public static BallsHolderSingleton Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new BallsHolderSingleton();
                }
                return instance;
            }
        }
    }

    public List<BallController> balls = new List<BallController>();
}

public class BallController : MonoBehaviour
{

    // This vector describes the direction that ball is moving in;
    // its length is equal to the ball's speed (the longer, the faster)
    public Vector3 direction = new Vector3(0f, 0f, 2.5f);

    // Effect flags that change the appearance of the ball
    public bool isAutoPilot = false;
    public bool isPiercing = false;
    public bool isSpeedChanged = false;

    [ColorUsageAttribute(false, true)]
    public Color colorAuto;

    [ColorUsageAttribute(false, true)]
    public Color colorPiercing;

    [ColorUsageAttribute(false, true)]
    public Color colorSpeed;

    private GameObject player;

    private Material material;
    private Color colorOriginal;
    private Material trailMaterial;
    private Color colorOriginalTrail;

    private AudioSource hitSound;

    void Awake()
    {
        // Get the player object (so that we know their position at all times)
        player = GameObject.FindGameObjectWithTag("Player");
        BallsHolderSingleton.Instance.balls.Add(this);

        // Access other components for rendering and sound
        //get the material and the original color, so that the color can be changed later.
        material = GetComponent<Renderer>().material;
        colorOriginal = material.GetColor("_EmissionColor");
        
        //gets the trail material and original color
        trailMaterial = GetComponent<TrailRenderer>().material;
        colorOriginalTrail = trailMaterial.GetColor("_EmissionColor");

        hitSound = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        // Move along the current direction
        transform.position += direction * Time.deltaTime;
    }

    private void Update()
    {
        SetColor();
    }

    private void OnDisable()
    {
        BallsHolderSingleton.Instance.balls.Remove(this);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.contactCount > 0)
        {
            ContactPoint contactPoint = collision.contacts[0];

            if (collision.collider.CompareTag("Platform"))
            {
                HandlePlatformCollision(collision.collider, contactPoint);
            }
            else
            {
                hitSound.Play();
                
                // When colliding with a brick, perform some additional calculations
                bool isBrick = collision.collider.CompareTag("Brick");
                if (isBrick)
                {
                    if (collision.collider.GetComponent<AutoPilotEffect>() != null)
                    {
                        // check if there is an auto-pilot effect attached to that brick.
                        // If this is the case, immediately switch on the auto pilot mode
                        this.isAutoPilot = true;
                    }
                    else if (collision.collider.GetComponent<PiercingEffect>() != null)
                    {
                        // check if there is an auto-pilot effect attached to that brick.
                        // If this is the case, immediately switch on the auto pilot mode
                        this.isPiercing = true;
                    }
                }

                // During auto pilot, reflect the ball back along the normal vector
                // and don't send it back to the player
                if (this.isPiercing)
                {
                    // While in piercing mode, bricks should not change the ball's direction at all.
                    // Only return it when it isn't a brick
                    if (!isBrick)
                    {
                        ReturnToPlayer(contactPoint);
                    }
                }
                else if (this.isAutoPilot)
                {
                    // While in auto-pilot, reflect normally and don't force the ball back to the player
                    ReflectNormally(contactPoint);
                }
                else
                {
                    ReturnToPlayer(contactPoint);
                }
            }
        }
    }

    private void SetColor()
    {
        //Checks in wich State the Ball is and change the color based on that.
        //Not the base color is changed, but the emission color
        if (isSpeedChanged)
        {
            material.SetColor("_EmissionColor", colorSpeed);
            trailMaterial.SetColor("_EmissionColor", colorSpeed);
        }
        else if (isAutoPilot)
        {
            material.SetColor("_EmissionColor", colorAuto);
            trailMaterial.SetColor("_EmissionColor", colorAuto);
        }
        else if (isPiercing)
        {
            material.SetColor("_EmissionColor", colorPiercing);
            trailMaterial.SetColor("_EmissionColor", colorPiercing);
        }
        else
        {
            material.SetColor("_EmissionColor", colorOriginal);
            trailMaterial.SetColor("_EmissionColor", colorOriginalTrail);
        }
    }

    private void HandlePlatformCollision(Collider collider, ContactPoint contactPoint)
    {
        // The more off-center the collision with the platform,
        // the more twisted the normal vector of the reflection becomes.
        // The new normal vector is calculated relative to the platform's size
        // and how far away from the center the collision occurred.
        float platformWidth = collider.bounds.extents.x;
        Vector3 platformCenterPosition = collider.transform.position;
        Vector3 contactPosition = contactPoint.point;
        Vector3 centerNormal = contactPoint.normal.normalized * platformWidth * 2f;
        Vector3 reflectionAxis = (platformCenterPosition - centerNormal) - contactPosition;
        Vector3 normal = reflectionAxis.normalized;

        // When the ball collides with the platform,
        // change its direction using accurate physics
        // (https://math.stackexchange.com/a/13263)
        // Calculate reflection vector and ball's new direction
        Vector3 reflection = direction - 2 * (Vector3.Dot(direction, normal)) * normal;
        this.direction = reflection;
    }

    private void ReflectNormally(ContactPoint contactPoint)
    {
        // Change direction using accurate physics
        // (https://math.stackexchange.com/a/13263)
        // Calculate reflection vector and ball's new direction
        Vector3 normal = contactPoint.normal;
        this.direction = direction - 2 * (Vector3.Dot(direction, normal)) * normal;
    }

    private void ReturnToPlayer(ContactPoint contactPoint)
    {
        // When the ball collides with a brick, always reflect it back towards the player,
        // no matter what the previous direction was. Keep the previous speed however,
        // so normalize the reflected vector and scale it up again
        Vector3 reflection = player.transform.position - contactPoint.point;
        float speed = direction.magnitude;
        this.direction = reflection.normalized * speed;
    }
}
