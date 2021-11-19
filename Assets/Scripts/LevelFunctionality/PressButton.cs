using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressButton : MonoBehaviour, InteractiveObject
{
    public GameObject[] gameObjects;
    public void Interact()
    {
        foreach (GameObject obj in gameObjects)
        { 
            //obj.GetComponent<>
        }
    }
}
