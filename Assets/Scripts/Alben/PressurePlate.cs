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

    private bool isActive;

    /// <summary>
    /// Start at frame one the isActive Boolean variable that is used to avoid
    /// repetitive use of a method when the pressure plate is touched or not.
    /// </summary>
    private void Start()
    {
        isActive = false;
    }

    /// <summary>
    /// Check every frame if there is anything (other than the floor) that touches the pressure plate.
    /// </summary>
    private void Update()
    {
        // Make sure to remove the collider for the plate because that will be counted for the number of colliders it checks
        if (!isActive && Physics.OverlapBox(transform.position, transform.localScale / 2).Length > 1)
        {
            this.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
            foreach (GameObject pressureObject in pressureObjects)
            {
                pressureObject.GetComponent<PressureObject>().Activate();
            }
            isActive = true;
        }
        else if (isActive && Physics.OverlapBox(transform.position, transform.localScale / 2).Length <= 1)
        {
            this.gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
            foreach (GameObject pressureObject in pressureObjects)
            {
                pressureObject.GetComponent<PressureObject>().Deactivate();
            }
            isActive = false;
        }
    }
}
