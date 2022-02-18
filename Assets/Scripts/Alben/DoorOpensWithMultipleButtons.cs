using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script will have the assigned door to open after a certain number of buttons correlated to
/// it are pressed.
/// IMPORTANT: Be aware that the script is for the door itself while it's door pivot is a parent of the door.
/// Author: Alben Trang
/// </summary>
public class DoorOpensWithMultipleButtons : MonoBehaviour
{
    [Tooltip("Put the pivot on one side of the door to let it turn like a real door")]
    public GameObject doorPivot;
    public float rotationSpeed;
    [Tooltip("Number of buttons that need to be pressed to open the door")]
    public int buttonsToBePressed = 2;

    int numOfButtonsPressed;

    // Start is called before the first frame update
    private void Start()
    {
        numOfButtonsPressed = 0;
    }
    
    /// <summary>
    /// Use this method to count how many buttons are pressed.
    /// If a certain number of them are pressed, the door opens.
    /// </summary>
    public void ActivateDoor()
    {
        numOfButtonsPressed++;
        if (numOfButtonsPressed >= buttonsToBePressed)
        {
            StartCoroutine(RotateDoor(Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, -90, 0)));
        }
    }

    /// <summary>
    /// Rotates the door smoothly to go from one angle to another using a pivot.
    /// </summary>
    /// <param name="startAngle">The angle that the door starts at.</param>
    /// <param name="endAngle">The angle that the door ends at.</param>
    /// <returns>Yields null.</returns>
    private IEnumerator RotateDoor(Quaternion startAngle, Quaternion endAngle)
    {
        for (float slerpRate = 0.1f; slerpRate <= 1.1f; slerpRate += 0.1f)
        {
            doorPivot.transform.rotation = Quaternion.Slerp(startAngle, endAngle, slerpRate);
            yield return null;
        }
    }
}
