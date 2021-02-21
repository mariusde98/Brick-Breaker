using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPilotEffect : BrickEffect
{

    // Dieser BrickEffect sorgt dafür, dass der Ball für die Zeit duration nicht automatisch zum Spieler zurückfliegt, sondern nun frei fliegen kann, also auch von einer Wand direkt zur nächsten Abprallen kann.

    // How long is the effect active (in seconds)
    [Range(1f, 60f)]
    public float duration = 10;

    public override IEnumerator Apply(BallController ballController)
    {
        // Turn on auto pilot
        ballController.isAutoPilot = true;
        
        // Wait for the effect to stop
        yield return new WaitForSeconds(duration);

        // Turn off auto pilot again
        ballController.isAutoPilot = false;
    }
}
