using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

[System.Serializable]
public class ScoreUpdateEvent : UnityEvent<int> { }

public class ScoreManager : MonoBehaviour
{

    // Der Score Manager verwaltet alles was mit dem Erfassen des Punktestands zutun hat. Er reagiert auf verschiedene LevelEvents, um etwa den Punktestand bei einer neuen Runde zusüsetzen zu können.

    public int scoreOfABlock;
    public ScoreUpdateEvent onScoreUpdateEvent;

    //Reference to the Score Savestate
    private ScoreSavestate scoreSavestate = new ScoreSavestate();

    private int currentScore; //score of the current level
    private float timer;

    private void Awake()
    {
        //Trying to load the scoreSavestate if one was already saved
        SaveLoadManager.LoadObject(scoreSavestate);
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(scoreSavestate.highScore);
        Debug.Log(new DateTime(scoreSavestate.timeStamp).ToString());
    }

    private void Update()
    {
        if (LevelEvent.state == LevelEvent.STATE_PLAYING)
        {
            //Adding time to the timer if player is playing
            timer += Time.unscaledDeltaTime;
        }
    }

    public void OnLevelEvent(int levelEvent)
    {
        if (levelEvent == LevelEvent.LEVEL_START)
        {
            OnLevelStart();
        }
        else if (levelEvent == LevelEvent.LEVEL_STOP)
        {
            OnLevelFinished();
        }
    }


    //Gets called by the BlockGeneratorScript Event when a block is destroyed
    public void OnBlockDestroyed(BrickEffect[] effects, BallController ballController)
    {
        Debug.Log("ScoreManager : OnBlockDestroyed() Event call");
        currentScore += scoreOfABlock;
        onScoreUpdateEvent.Invoke(currentScore);
    }

    private void OnLevelStart()
    {
        currentScore = 0;
        timer = 0;
        onScoreUpdateEvent.Invoke(currentScore);
    }

    private void OnLevelFinished()
    {
        if (currentScore > scoreSavestate.highScore)
        {
            Debug.Log("ScoreManager : new HighScore");

            //Got a new highscore
            scoreSavestate.highScore = currentScore;
            //Getting the current Time
            scoreSavestate.timeStamp = System.DateTime.Now.Ticks;
            scoreSavestate.time = timer;

            SaveLoadManager.SaveObject(scoreSavestate);
        }
    }

    public ScoreSavestate GetHighscore()
    {
        return scoreSavestate;
    }
    public int GetCurrentScore()
    {
        return currentScore;
    }
    public float GetTime()
    {
        return timer;
    }

    private void OnDisable()
    {
        //Saving the highscore
        SaveLoadManager.SaveObject(scoreSavestate);
    }
}
