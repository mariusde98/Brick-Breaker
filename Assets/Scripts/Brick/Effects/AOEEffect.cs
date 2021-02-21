using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEEffect : BrickEffect
{

    // Dieser Effekt löst einen Explosionseffekt aus. Bricks im Umkreis des effectRadius werden bei Zerstörung des Bricks mit diesem BrickEffect mit ihm zerstört

    public float effectRadius = 3f;

    public override IEnumerator Apply(BallController ballController)
    {
        // Perform a sphere cast around the center of this effect's object.
        // Any other BrickController in this sphere will be obliterated
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, effectRadius, transform.forward);
        foreach (RaycastHit hit in hits)
        {
            // Make sure that this object is actually inside the sphere
            // (its "distance" is set to 0 if that's the case)
            if (hit.distance == 0)
            {
                BrickController otherBrick = hit.collider.gameObject.GetComponent<BrickController>();
                if (otherBrick != null)
                {
                    otherBrick.DestroyBlock();
                }
            }
        }
        yield return null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, effectRadius);
    }
}
