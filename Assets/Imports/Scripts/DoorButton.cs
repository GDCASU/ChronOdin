using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Whenever this button is pressed by the player, the assigned door is opened.
/// </summary>
public class DoorButton : MonoBehaviour
{
    public GameObject door;                     //This is a reference to the door we want to open that we will be able to set on the editor

    /// <summary>
    /// Opens the door when the player collides with the button by trigger.
    /// </summary>
    /// <param name="other">The game object colliding with the button</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            door.GetComponent<MoveDoor>().Move();
        }
    }
}
