using UnityEngine;

public class ScoreSavestate : Saveable
{
    //Setting the path of this class
    public ScoreSavestate() : base("score") { }

    //Highest score
    [SerializeField]
    public int highScore;

    //When the highest score was archived
    [SerializeField]
    public long timeStamp;

    //Time of the level
    [SerializeField]
    public float time;

}
