using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawnScript : MonoBehaviour
{
    public GameObject ballPrefab;

    public void OnLevelEvent(int levelEvent)
    {
        if (levelEvent == LevelEvent.LEVEL_START)
        {
            SpawnBall(transform.position, Vector3.zero);
        }
    }

    public void SpawnBall(Vector3 position, Vector3 direction)
    {
        GameObject newObject = Instantiate(ballPrefab, position, Quaternion.identity);

        // Apply the desired direction too
        BallController controller = newObject.GetComponent<BallController>();
        if (direction != Vector3.zero && controller != null)
        {
            float speed = controller.direction.magnitude;
            controller.direction = direction.normalized * speed;
        }
    }
}
