using System.Collections;
using UnityEngine;

/// <summary>
/// This script will have the assigned door open or close from the player's interaction.
/// IMPORTANT: Be aware that the script is for the door itself while it's door pivot is a parent of the door.
/// Author: Alben Trang
/// </summary>
public class Door : SimpleTimeManipulation
{
    [Tooltip("Put the pivot on one side of the door to let it turn like a real door")]
    public GameObject doorPivot;
    public float rotationSpeed;

    private bool isOpen, isMoving;

    /// <summary>
    /// Start at frame one to set the initial Boolean variables.
    /// </summary>
    private void Start()
    {
        UpdateTimescale(MasterTime.singleton.timeScale);
        isOpen = false;
        isMoving = false;
    }

    /// <summary>
    /// Opens or closes the door when the player clicks on it.
    /// </summary>
    public void InteractWithDoor()
    {
        if (!isMoving)
        {
            isMoving = true;

            if (!isOpen)
            {
                StartCoroutine(RotateDoor(Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, -90, 0)));
                isOpen = true;
            }
            else
            {
                StartCoroutine(RotateDoor(Quaternion.Euler(0, -90, 0), Quaternion.Euler(0, 0, 0)));
                isOpen = false;
            }

            isMoving = false;
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
        float slerp = 0;
        float slerpRate = Time.deltaTime * timeScale * rotationSpeed * .1f;
        while (slerpRate <= 1)
        {
            slerpRate = Time.deltaTime * timeScale * rotationSpeed * .1f;
            doorPivot.transform.rotation = Quaternion.Slerp(startAngle, endAngle, slerpRate);
            slerp += slerpRate;
            yield return null;
        }
    }
}
