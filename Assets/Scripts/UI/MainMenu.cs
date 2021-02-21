using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MainMenu : MonoBehaviour
{

    // Das Hauptmenü kann auf Level Events reagieren. Wenn UI Elemente auf diese Level Events reagieren sollen sind diese funktionen hier in der Funktion onLevelEvent zu implementieren
    public GameObject endScreen;

    public void OnLevelEvent(int levelEvent)
    {
        switch (levelEvent)
        {
            case LevelEvent.LEVEL_START:
                endScreen.SetActive(false);
                break;
            case LevelEvent.LEVEL_PLAY:
                break;
            case LevelEvent.LEVEL_PAUSE:
                break;
            case LevelEvent.LEVEL_STOP:
                endScreen.SetActive(true);
                break;
        }
    }
}
