using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LoadIntroScene : MonoBehaviour
{
    public Color onColor;
    public Color offColor;
    public Color waitColor;

    public Image gazeImg;
    private bool gaze = false;
    private float timer;
    public float duration;
    public GameManager manager;

    public UnityEvent myEvent;
    public string label;

    private TextMesh myTextMesh;

    // Start is called before the first frame update
    void Start()
    {
        myTextMesh = GetComponentInChildren<TextMesh>();
        // myTextMesh.text = label;

        ChangeColor(offColor);
    }

    // Update is called once per frame
    void Update()
    {
        if (gaze)
        {
            if (duration > timer)
            {
                timer += Time.unscaledDeltaTime;
                gazeImg.fillAmount = timer / duration;
            }
            else
            {
                On();
            }
        }
    }

    private void ChangeColor(Color color)
    {
        GetComponent<Renderer>().material.SetColor("_BaseColor", color);
    }

    public void On()
    {

        gaze = false;
        gazeImg.fillAmount = 0;

        ChangeColor(onColor);
        //myEvent.Invoke();
        SceneManager.LoadScene(0);
    }
    public void Off()
    {
        ChangeColor(offColor);

        if (gaze)
        {
            gaze = false;
            gazeImg.fillAmount = 0;
        }
    }
    public void StartCount()
    {
        timer = 0;
        gaze = true;
        ChangeColor(waitColor);
    }
}
