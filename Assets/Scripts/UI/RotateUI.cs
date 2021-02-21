using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateUI : MonoBehaviour
{

    // Dieses Skript dient dazu UI Elemente stets im Blickfeld des Spielers zu halten. Es soll die UI Elemente Smooth um eine bestimmte Achse rotieren
    // Aktuell sind die Funktionen dieses Skripts auskommentiert, da es uns in der Zeit nicht gelang alle Bugs zu beheben

    private Transform cameraTransform;
    private GameObject cam;
    private Vector3 axis = new Vector3(0, 1, 0);
    private float angle;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        cameraTransform = cam.transform;
    }
}
