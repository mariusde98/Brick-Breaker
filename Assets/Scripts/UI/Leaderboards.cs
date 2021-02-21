using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Leaderboards : MonoBehaviour
{
    // Diese Klasse dient zum aktualisieren des TextMeshes, welches den aktuellen Highscore in korrekter Formatierung darstellen soll. Hierzu wird der Highscore vom ScoreManager angefragt
    public Text myText;
    public ScoreManager score;

    private void OnEnable()
    {
        SetText();
    }

    public void SetText()
    {
        // set Leaderboards text to current Highscore
        if (score.GetHighscore().highScore >= score.GetCurrentScore())
        {
            string date = new System.DateTime(score.GetHighscore().timeStamp).ToString();
            myText.text = "The Highscore: " + score.GetHighscore().highScore + " \n Seconds Survived : " + Mathf.Round(score.GetHighscore().time) + " \n Date: " + date;
        }
        else
        {
            myText.text = "The Highscore: " + score.GetCurrentScore();
        }
    }

}
