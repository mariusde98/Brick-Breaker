using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;


[System.Serializable]
public class LevelEvent : UnityEvent<int>
{

    // Level Events können über den Inspektor in Unity hinzugefügt werden. Es sind 4 verschiedene Events (Start, Play, Pause und Stopp) verfügbar, welche in den einzelnen Scripts verwendet werden können indem man sie über den Insprektor am GameManager Objekt registiert.

    public const int LEVEL_START = 0, //When level is starting, needs to be reset
    LEVEL_PLAY = 1, //When level is playing again, after a pause
    LEVEL_PAUSE = 2, //Pause when level is pausing but not stopping
    LEVEL_STOP = 3; //Level finished => player died

    //Level States
    public static int state = 1; //State in wish the player is starting in

    public const int STATE_PLAYING = 0,
    STATE_IN_MENU = 1;

    public new void Invoke(int value)
    {
        //Setting the state debending on the event
        switch (value)
        {
            case LEVEL_START:
                state = STATE_PLAYING;
                break;
            case LEVEL_PLAY:
                state = STATE_PLAYING;
                break;
            case LEVEL_PAUSE:
                state = STATE_IN_MENU;
                break;
            case LEVEL_STOP:
                state = STATE_IN_MENU;
                break;
        }

        base.Invoke(value);
    }
}


public class GameManager : MonoBehaviour
{

    // Der GameManager verwaltet den Zustand des Spiels und des User Interfaces. Hier werden die meisten Spielelemente registriert und genutzt. Hier finden sich auch jene Funktionen wieder, welche bei Levelevents ausgelöst werden. 
    // Zu diesen Funktionen gehören gamePlay() gameStart() gameStop() gamePause(). Ebenfalls wird in diesem Skript auch die steigende Schwierigkeit im Spiel verwaltet

    public LevelEvent onLevelEvent;
    private int hits;
    private float normalGameSpeed;
    private float gameSpeed;

    [Header("Difficulty Settings")]
    [Tooltip("Time in seconds until the next sphere hull should spawn and ball gets faster")]
    public float timeToNextDifficulty;
    [Tooltip("The x-Axis is the time and the y-axis is the speed incrase")]
    public AnimationCurve speedOverTime;

    [Header("References")]
    public PlatformController platform;
    public ScoreManager score;
    public BlockGeneratorScript blockGenerator;

    // UI
    public GameObject leaderboardsObject;
    public Leaderboards leaderboards;
    public GameObject bottomMenuCylinder;
    public GameObject bottomMenu;
    public GameObject startMenu;


    private bool isDifficultyCRRunning;

    // Start is called before the first frame update
    void Start()
    {
        hits = 0;
        normalGameSpeed = Time.timeScale;
        gameSpeed = normalGameSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = gameSpeed;
    }

    public IEnumerator UpdateDifficulty()
    {
        isDifficultyCRRunning = true;
        while (!blockGenerator.IsReachedLastLayer() && LevelEvent.state == LevelEvent.STATE_PLAYING)
        {
            yield return new WaitForSecondsRealtime(timeToNextDifficulty / 10f);

            float time = score.GetTime();

            int difficulty = (int)(time / timeToNextDifficulty);

           // Debug.Log("Current Time:" + time + " Current Difficulty:" + difficulty);

            //Incrase Ball speed over time
            BallSpeedEffect speedIncrase = this.gameObject.GetComponent<BallSpeedEffect>();
            if (!speedIncrase)
            {
                speedIncrase = this.gameObject.AddComponent<BallSpeedEffect>();
            }
            speedIncrase.duration = timeToNextDifficulty / 10f;
            speedIncrase.speedFactor = speedOverTime.Evaluate(time) + 1;


            List<BallController> balls = BallsHolderSingleton.Instance.balls;
            foreach (BallController ballC in balls)
            {
                StartCoroutine(ExecuteBlockEffect(speedIncrase, ballC));
            }

            blockGenerator.UpdateSphereLayers(difficulty);
        }
        isDifficultyCRRunning = false;
    }


    public void ToggleBottomMenu()
    {
        bottomMenu.SetActive(!bottomMenu.activeSelf);

        if (bottomMenu.activeSelf)
        {
            GamePause();
        }
        else
        {
            GamePlay();
        }

        // Hide startMenu if it was active
        if (startMenu.activeSelf == true)
        {
            startMenu.SetActive(false);
        }

        // Hide leaderboards if it was active
        if (leaderboardsObject.activeSelf == true)
        {
            leaderboardsObject.SetActive(false);
        }
    }

    public void ToggleStartMenu()
    {
        startMenu.SetActive(!startMenu.activeSelf);
    }
    public void ToggleLeaderboards()
    {
        leaderboardsObject.SetActive(!leaderboardsObject.activeSelf);
    }

    // Getters & Setters
    public int GetHits()
    {
        return hits;
    }

    // Call this when the player hit another brick
    public void Hit()
    {
        hits++;
    }

    public float GetGameSpeed()
    {
        return gameSpeed;
    }

    public void SetGameSpeed(float gameSpeed)
    {
        this.gameSpeed = gameSpeed;
    }

    //LEVEL STATES AND STUFF

    [ContextMenu("GameStart()")]
    public void GameStart()
    {
        Debug.Log("GameManager : GameStart()");

        //Destroying all balls
        BallController[] balls = BallsHolderSingleton.Instance.balls.ToArray();
        foreach (BallController ballC in balls)
        {
            GameObject.Destroy(ballC.gameObject);
        }

        bottomMenu.SetActive(false);
        startMenu.SetActive(false);
        leaderboardsObject.SetActive(false);
        bottomMenuCylinder.SetActive(true);

        SetGameSpeed(normalGameSpeed);

        //resetting all the objects so the player can play again
        //BlockMeshGen is will reset in the BLockGeneratorScript
        onLevelEvent.Invoke(LevelEvent.LEVEL_START);

        // After the initial start, immediately transition into the playing state
        GamePlay();
    }

    [ContextMenu("GamePlay()")]
    public void GamePlay()
    {
        Debug.Log("GameManager : GamePlay()");

        SetGameSpeed(normalGameSpeed);

        //unpausing the gampleay if it is paused
        onLevelEvent.Invoke(LevelEvent.LEVEL_PLAY);

        // Start the coroutine that will make the game harder
        // (this needs to be in gamePlay() rather than gameStart(),
        // since otherwise it might not get restarted after closing the menu)
        if (!isDifficultyCRRunning)
        {
            StartCoroutine(UpdateDifficulty());
        }
    }

    [ContextMenu("GamePause()")]
    public void GamePause()
    {
        Debug.Log("GameManager : GamePause()");
        SetGameSpeed(0f);

        //Pausing the current gameplay
        onLevelEvent.Invoke(LevelEvent.LEVEL_PAUSE);
    }

    [ContextMenu("GameStop()")]
    public void GameStop()
    {
        Debug.Log("GameManager : GameStop()");
        //stopping the gamplay and opening the scoreboard/menu
        onLevelEvent.Invoke(LevelEvent.LEVEL_STOP);
        //SceneManager.LoadScene(0);
    }

    public void Reload()
    {
        GameStop();
        GameStart();
    }

    // This method is connected to the OnDestroyed event of a BrickController
    // generated in the game. When called, it will schedule all of the provided
    // effects of that brick to be played
    public void OnBlockDestroyed(BrickEffect[] effects, BallController ballController)
    {
        foreach (BrickEffect effect in effects)
        {
            StartCoroutine(ExecuteBlockEffect(effect, ballController));
        }
    }

    private IEnumerator ExecuteBlockEffect(BrickEffect effect, BallController ballController)
    {
        // TODO Update UI (show icon for the effect)

        yield return effect.Apply(ballController);

        // TODO Update UI (remove icon for the effect)
    }

    private void OnDestroy()
    {
        onLevelEvent.RemoveAllListeners();
    }
}
