using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BrickDestroyedEvent : UnityEvent<BrickEffect[], BallController> { }

public class BrickController : MonoBehaviour
{

    // Whether or not the brick can be destroyed by the ball
    public bool invincible = false;

    // The effect to play when the brick is destroyed
    public GameObject destroyEffectPrefab;

    // This event is sent when the brick is destroyed by the ball
    public BrickDestroyedEvent onDestroyed;

    // List of effects to trigger when the brick is destroyed
    private BrickEffect[] effects;


    void Start()
    {
        // Collect all scripts attached to this GameObject, which are a subclass of "BrickEffect"
        effects = GetComponents<BrickEffect>();
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Ball"))
        {
            // Destroy the brick upon collision, unless it cannot be destroyed
            if (invincible)
            {
                return;
            }

            // Notify listeners about all the destruction
            onDestroyed.Invoke(effects, other.gameObject.GetComponent<BallController>());

            // Destroy the brick
            DestroyBlock();
        }
    }

    public void DestroyBlock()
    {
        // Play interaction effect (if any), then destroy yourself
        if (destroyEffectPrefab != null)
        {
            Instantiate(destroyEffectPrefab, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
}
