using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveLoadManager
{
    private static string fileFormat = ".dat";

    private static void CreateFolderStructure(string path)
    {
        //Checking if the folder structure exist, if not it will be created
        string[] subPaths = path.Split('/');
        if (subPaths.Length > 0)
        {
            string currentSubPath = "";
            for (int i = 1; i < subPaths.Length - 1; i++)
            {
                currentSubPath += "/" + subPaths[i];


                if (!Directory.Exists(Application.persistentDataPath + currentSubPath))
                {
                    Debug.Log("Folder didn't existed, will create new one path:" + currentSubPath);
                    Directory.CreateDirectory(Application.persistentDataPath + currentSubPath);
                }
            }
        }
    }


    public static void SaveObject(Saveable o)
    {
        string path = o.path;
        //Checking if the folder structure exist, if not it will be created
        CreateFolderStructure(path);


        BinaryFormatter bf = new BinaryFormatter();
        FileStream fSteam = File.Create(Application.persistentDataPath + "/" + path + fileFormat);
        var json = JsonUtility.ToJson(o);
        bf.Serialize(fSteam, json);
        fSteam.Close();
    }

    public static void LoadObject(Saveable o)
    {
        string path = o.path;
        //Checking if the folder structure exist, if not it will be created
        CreateFolderStructure(path);

        if (File.Exists(Application.persistentDataPath + "/" + path + fileFormat))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fStream = File.Open(Application.persistentDataPath + "/" + path + fileFormat, FileMode.Open);
            JsonUtility.FromJsonOverwrite((string)bf.Deserialize(fStream), o);
            fStream.Close();

        }
        else
        {
            Debug.Log("File doesnt exist at path:" + Application.persistentDataPath + "/" + path + fileFormat);
        }
    }
}
