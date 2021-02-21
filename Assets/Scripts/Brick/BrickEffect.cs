using System.Collections;
using UnityEngine;

// The base class for any kind of effect that can be attached to a brick.
public abstract class BrickEffect : MonoBehaviour
{
    // Apply the effect. This returns an "IEnumerator" so that it can be started asynchronously in a coroutine
    public abstract IEnumerator Apply(BallController ballController);
}
