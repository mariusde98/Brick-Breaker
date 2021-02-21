using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSizeEffect : BrickEffect
{

    // Dieser BrickEffect löst eine Veränderung der Plattformgröße aus. Dabei gibt es Bricks, welche entweder eine Vergrößerung, oder Verkleinerung der Plattform verursachen. Hierzu wird die Funktion setScale des Plattform Controllers aufgerufen

    // How much does this effect change the size (1.0 == no change)
    [Range(0.25f, 2f)]
    public float sizeFactor = 2;

    // How long is the effect active (in seconds)
    [Range(1f, 60f)]
    public float duration = 10;

    private PlatformController platformController;

    void Start()
    {
        // Locate the ball in the scene and store a reference to its BallController
        GameObject platform = GameObject.FindGameObjectWithTag("Platform");
        platformController = platform.GetComponent<PlatformController>();
    }

    public override IEnumerator Apply(BallController ballController)
    {
        // Update the platform's size
        platformController.SetScale(sizeFactor, sizeFactor);

        // Wait for the effect to stop
        yield return new WaitForSeconds(duration);

        // Reset the platform's size
        platformController.SetScale(1, 1);
    }
}
