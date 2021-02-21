using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SphereSettings", menuName = "States/SphereSettings")]
public class SphereSettings : ScriptableObject
{
    //Script for holding the settings of a Spherelayer for the BlockGenerator
    //To create a new level you can just create a new SphereSettings and change the settings


    //Settings
    [Header("Sphere Hull Settings")]
    [Tooltip("Block Count on the entire Sphere")]
    public int blockCount;

    [Tooltip("Radius of the sphere")]
    public float radius = 100f;


    [Tooltip("Clamp in X Direction")]
    [SerializeField]
    [MinMaxSlider(0f, 1f)]
    public Vector2 clampX = new Vector2(0f, 1f);

    [Tooltip("Clamp in Y Direction")]
    [SerializeField]
    [MinMaxSlider(0f, 1f)]
    public Vector2 clampY = new Vector2(0f, 1f);


    [Tooltip("Clamp in Z Direction")]
    [SerializeField]
    [MinMaxSlider(0f, 1f)]
    public Vector2 clampZ = new Vector2(0f, 1f);


    //Reference to the block prefabs that will be copied
    [Header("Block Settings")]
    [Tooltip("One of these blocks will be placed randomly if no special block got placed")]
    public GameObject[] defaultPrefabs;

    [Tooltip("Each block has its own chance to get placed")]
    public GameObject[] specialPrefabs;

    [Tooltip("Chance for each block that it will be placed (0.1 = 10%) NEEDS TO BE THE SAME LENGTH AS specialPrefabs"), Range(0, 1)]
    public float[] specialChance;


}
