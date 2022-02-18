using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is for a button that is touched by the player 
/// to add to the number of buttons needed to open a door.
/// 
/// Author: Alben Trang
/// </summary>
public class OneButtonForDoor : MonoBehaviour
{
    [Tooltip("Put the door object that requires multiple buttons to be pressed to open it")]
    public GameObject doorToActivate;

    private bool isPressed;

    private void Start()
    {
        isPressed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPressed && other.gameObject.CompareTag("Player"))
        {
            isPressed = true;
            doorToActivate.GetComponent<DoorOpensWithMultipleButtons>().ActivateDoor();
        }
    }
}
