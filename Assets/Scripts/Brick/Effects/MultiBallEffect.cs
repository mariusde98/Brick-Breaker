using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiBallEffect : BrickEffect
{

    // Durch diese BrickEffect spawnen zusätzliche Bälle. Allerdings hat der Spieler erst bei Verlust des letzten Balls verloren

    public int newCount = 3;
    public float radius = 1f;

    private BallSpawnScript ballSpawner;

    void Start()
    {
        GameObject spawnObject = GameObject.FindGameObjectWithTag("BallSpawn");
        if (spawnObject != null)
        {
            ballSpawner = spawnObject.GetComponent<BallSpawnScript>();
        }
    }

    public override IEnumerator Apply(BallController ballController)
    {
        if (ballSpawner != null && newCount > 0)
        {
            // Ask the spawner to create new balls.
            // Their direction is derived from that of the original ball,
            // but slightly altered using a circular cone shape.
            // The original position is the tip of the cone and the "radius" property defines
            // the radius of its base circle. Random points on that circle are sampled
            // and used as the direction of the created balls
            Vector3 position = ballController.transform.position;
            Vector3 direction = ballController.direction;
            Vector3 circleCenter = position + direction.normalized * radius;
            Vector3 crossDirection = Vector3.Cross(direction, Vector3.up).normalized * radius;

            float angleStep = 360 / newCount;
            for (int i = 0; i < newCount; i++)
            {
                // Calculate the end point of the new direction
                float angle = angleStep * i;
                Vector3 endPoint = Quaternion.AngleAxis(angle, direction) * crossDirection;
                Vector3 newDirection = endPoint - position;
                ballSpawner.SpawnBall(position, newDirection);
            }
        }
        yield return null;
    }
}
