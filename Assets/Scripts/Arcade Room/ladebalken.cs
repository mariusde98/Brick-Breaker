using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ladebalken : MonoBehaviour
{
    // Start is called before the first frame update
    public Color offColor;
    public Color onColor;
    public Color waitColor;
    private float timer;
    public float duration;
    public bool gaze;
    public Image gazePanel;
    public Camera mainCam;
    public Text gazeText;
    public float fov = 60;
    private bool inGame;
    public GameObject coin;
    public GameObject startText;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;
        duration = 2.0f;
        gazePanel.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        inGame = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (gaze)
        {
            if (duration > timer)
            {
                startText.SetActive(true);
                timer += Time.deltaTime;
                gazePanel.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, timer * 175);
                print("duration: " + duration + "    Timer: " + timer);
            }
            else
            {
                timer = 0f;
                gaze = false;
                StartCoroutine(waiter());
            }
        }
    }

    IEnumerator waiter()
    {
        inGame = true;
        gazePanel.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        float elapsedTime = 0;
        float waitTime = 3.5f;
        while (elapsedTime <= waitTime)
        {
            elapsedTime += Time.deltaTime;
            coin.SetActive(true);
            mainCam.fieldOfView = 60 - (elapsedTime * 5 * (60 / mainCam.fieldOfView));
            
            yield return null;
        }
        SceneManager.LoadScene(1);
    }

    public void Off()
    {
        if (gaze)
        {
            gaze = false;
            gazePanel.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
            startText.SetActive(false);
        }
    }
    public void StartCount()
    {
        timer = 0;
        gaze = true;
    }
}
