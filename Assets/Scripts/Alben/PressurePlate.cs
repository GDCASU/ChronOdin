using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is for game object that is touched by something, like the player or
/// a ball, to activate objects assigned to it.
/// 
/// Author: Alben Trang
/// </summary>
public class PressurePlate : MonoBehaviour
{
    [Tooltip("Put in any objects with a script deriving from the PressureObject interface script to be activated and deactivated by the plate")]
    public GameObject[] pressureObjects;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            foreach (GameObject pressureObject in pressureObjects) pressureObject.GetComponent<LinkedToPressurePlate>().Activate();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            foreach (GameObject pressureObject in pressureObjects) pressureObject.GetComponent<LinkedToPressurePlate>().Deactivate();
        }
    }
}