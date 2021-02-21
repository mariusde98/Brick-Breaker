using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpeedEffect : BrickEffect
{

    // Dieser BrickEffect erhöht die Geschwindigkeit des Balls für eine gewisse Zeit

    // How much does this effect change the speed (1.0 == no change)
    [Range(0.1f, 20f)]
    public float speedFactor = 2;

    // How long is the effect active (in seconds)
    [Range(1f, 60f)]
    public float duration = 10;

    // Whether or not this effect should change the ball's color as well
    // (this should be reserved to Speed Effect blocks)
    public bool willChangeColor = false;

    public override IEnumerator Apply(BallController ballController)
    {
        // Scale the ball's direction vector by the given amount to change its speed
        float normalSpeed = ballController.direction.magnitude;
        ballController.direction = ballController.direction * speedFactor;

        if (willChangeColor)
        {
            ballController.isSpeedChanged = true;
        }

        // Wait for the effect to stop
        yield return new WaitForSeconds(duration);

        // Restore the original speed
        ballController.direction = ballController.direction.normalized * normalSpeed;

        if (willChangeColor)
        {
            ballController.isSpeedChanged = false;
        }
    }
}
