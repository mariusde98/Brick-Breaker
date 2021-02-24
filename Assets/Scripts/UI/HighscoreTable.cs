using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighscoreTable : MonoBehaviour
{
    private Transform entryContainer;
    private  ScoreManager sm;
    private Transform entryTemplate; 
    private List<Transform> highscoreEntryTransformList;
  

    
    private void Start(){
       
        entryContainer = transform.Find("highscoreEntryContainer");
        entryTemplate = entryContainer.Find("highscoreEntryTemplate");

        entryTemplate.gameObject.SetActive(false);
       
        sm = new ScoreManager ();
    

      /*  highscoreEntryList = new List<HighscoreEntry>() {
         new HighscoreEntry{score = 10},
         new HighscoreEntry{score = 100},
         
        };*/




      string jsonString = PlayerPrefs.GetString("highscoreTable");
       Highscores highscores =JsonUtility.FromJson<Highscores>(jsonString);
        /*simple Sorting Algorithem. We iterate Through the 
        highscoreEntryList and compare each position with each other.*/
     
        for(int i = 0; i < highscores.highscoreEntryList.Count; i++){
            for(int j = i + 1; j <  highscores.highscoreEntryList.Count; j++){
                if(highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score){
                    HighscoreEntry tmp = highscores.highscoreEntryList[i];
                   highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                    highscores.highscoreEntryList[j] = tmp;
                }
            }
        }

        highscoreEntryTransformList = new List<Transform>();
        foreach(HighscoreEntry highscoreEntry in highscores.highscoreEntryList){
            CreateHighscoreEntryTransform(highscoreEntry,entryContainer, highscoreEntryTransformList);

        }

    
   
    }

    private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList){
        
        float templateHeight = 1f;
         Transform entryTransform = Instantiate(entryTemplate, container);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
            entryTransform.gameObject.SetActive(true);

            int rank = transformList.Count + 1;
            string rankString;
            switch(rank){
                default: rankString = rank + "TH"; break;
                case 1: rankString = "1ST"; break;
                case 2: rankString = "2ND"; break;
                case 3: rankString = "3RD"; break;
            }
            
            int score = highscoreEntry.score;

            entryTransform.Find("posText").GetComponent<Text>().text = rankString;
            entryTransform.Find("scoreText").GetComponent<Text>().text = score.ToString();

            transformList.Add(entryTransform);


    }

    private void AddHighscoreEntry(int score, string name){
       HighscoreEntry highscoreEntry = new HighscoreEntry{ score = score};
        
       string jsonString = PlayerPrefs.GetString("highscoreTable");
       Highscores highscores =JsonUtility.FromJson<Highscores>(jsonString);
       highscores.highscoreEntryList.Add(highscoreEntry);

       string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();
       
    }

    private class Highscores{
        public List<HighscoreEntry> highscoreEntryList;
    }

    [System.Serializable]
    private class HighscoreEntry {
        public int score;
    }
}
