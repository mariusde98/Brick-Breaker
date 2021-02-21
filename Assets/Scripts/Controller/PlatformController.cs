using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{

    // Dieses Skript dient zur Kontrolle der Plattform, welche der Spieler durch seine Kopfbewegungen steuert. Durch Levelevents kann die Plattform auf verschiedene Ergeinisse und zustände während einer Runde reagieren

    public Material regularMaterial;
    public Material hitMaterial;
    public Transform cameraTransform;
    public float distance = 15;
    private bool hit = false;
    private Transform startTransform;
    public float smoothTime = 0.1F;
    private Vector3 velocity = Vector3.zero;
    private float currentScaleX = 1f;
    private float currentScaleY = 1f;
    private Renderer myRenderer;
    private TextMesh pointText;
    private AudioSource hitSound;

    void Start()
    {
        // Set material
        myRenderer = GetComponent<Renderer>();
        myRenderer.material = regularMaterial;
        hitSound = GetComponent<AudioSource>();

        startTransform = transform;

        Vector3 target = cameraTransform.position + cameraTransform.forward * distance;
        transform.position = target;

        //Hiding this Platform, Showing when level starts
        myRenderer.enabled = false;

        // Get pointText
        pointText = GetComponentInChildren<TextMesh>();
        SetPointText(0);
    }


    // Update is called once per frame
    void Update()
    {
        FollowGaze();
    }

    void FollowGaze()
    {
        Vector3 target = cameraTransform.position + cameraTransform.forward * distance;
        transform.rotation = cameraTransform.rotation;

        // apply movement
        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime);
    }

    // Change the platform's scale
    public void SetScale(float scaleX, float scaleY)
    {
        this.currentScaleX = scaleX;
        this.currentScaleY = scaleY;

        transform.localScale = new Vector3(scaleX, scaleY, 1);
    }

    private void OnCollisionEnter(Collision other)
    {
        //Debug.Log("Platform was hit");

        if (other.collider.gameObject.tag == "Ball")
        {
            Debug.Log("Player hit the platform");
            StartCoroutine(OnPlatformHit());
        }
    }

    private IEnumerator OnPlatformHit()
    {
        hitSound.Play();
        SwitchMaterial();
        yield return new WaitForSeconds(0.1f);
        SwitchMaterial();
    }

    public void SwitchMaterial()
    {
        if (hit)
        {
            myRenderer.material = regularMaterial;
            hit = false;
        }
        else
        {
            myRenderer.material = hitMaterial;
            hit = true;
        }
    }

    // This function refreshes the Points indicator on the plattform and is being called by the game manager using an event
    public void SetPointText(int points)
    {
        Debug.Log("setPointText:" + points);
        pointText.text = "" + points;
    }

    public void OnLevelEvent(int levelEvent)
    {
        switch (levelEvent)
        {
            case LevelEvent.LEVEL_START:
                ToggleRenderer(true);
                break;
            case LevelEvent.LEVEL_PLAY:
                ToggleRenderer(true);
                break;
            case LevelEvent.LEVEL_PAUSE:
                ToggleRenderer(false);
                break;
            case LevelEvent.LEVEL_STOP:
                ToggleRenderer(false);
                break;
        }
    }

    private void ToggleRenderer(bool isOn)
    {
        myRenderer.enabled = isOn;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = isOn;
        }
    }
}
