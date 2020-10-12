using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour {
 
    Text label;
    public GameController_2048Bricks SpeedLevel;

    void Start()
    {
        SpeedLevel = GameObject.Find("2048Bricks_Tap").GetComponent<GameController_2048Bricks>();
        

        Debug.Log(SpeedLevel.speed);
    }

    void Update ()
    {
        label.text = "asd";
    }
}