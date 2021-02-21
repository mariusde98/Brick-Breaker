using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCoinSound : MonoBehaviour
{
    public AudioClip coinSound;
    public AudioSource audioSource;
    public float volume = 0.1f;

    void InsertCoin()
    {
        audioSource.PlayOneShot(coinSound, volume);
    }
}
