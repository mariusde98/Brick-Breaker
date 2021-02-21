using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZoneScript : MonoBehaviour
{

    // Sobald der Letzte verfügbare Ball diese Zone kollidiert hat der Spieler die Runde verloren.
    private static readonly Color SPHERE_COLOR = new Color(1f, 0f, 0f, 0.3f);
    public GameManager gameManager;

    private AudioSource deathSound;

    void Awake()
    {
        deathSound = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ball")
        {
            //Ball entered the death zone -> delete ball
            GameObject.Destroy(other.gameObject);

            if (BallsHolderSingleton.Instance.balls.Count < 1)
            {
                //All balls are dead, call dead event in game manager
                deathSound.Play();
                gameManager.GameStop();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = SPHERE_COLOR;
        Gizmos.DrawSphere(this.transform.position, GetComponent<SphereCollider>().radius);
    }
}
