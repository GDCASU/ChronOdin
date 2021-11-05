using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object that is moved when the pressure plate is pressed or down when the plate
/// is not pressed.
/// Author: Alben Trang
/// </summary>
public class PressureMovableObject : MonoBehaviour, PressureObject
{
    [Header("New Position Coordinates")]
    [Tooltip("The objects new x position")]
    public float moveX = 5.0f;
    [Tooltip("The objects new y position")]
    public float moveY = 5.0f;
    [Tooltip("The objects new z position")]
    public float moveZ = 5.0f;

    [Header("Activation Delays")]
    [Tooltip("Set how many seconds before the object is activated")]
    public float activationDelay = 1.0f;
    [Tooltip("Set how many seconds before the object is deactivated (Tip: if the pressure plate is pressed briefly, set this higher than activationDelay for better effect)")]
    public float deactivationDelay = 3.0f;

    private Vector3 originalPosition;
    private Vector3 newPosition;

    /// <summary>
    /// Start at frame one to store the object's original position and it's new position.
    /// </summary>
    private void Start()
    {
        originalPosition = this.transform.position;
        newPosition = new Vector3(transform.position.x + moveX, transform.position.y + moveY, transform.position.z + moveZ);
    }

    /// <summary>
    /// Moves the object when the pressure plate is pressed.
    /// </summary>
    public void Activate()
    {
        StopCoroutine(ReturnObject());
        StartCoroutine(MoveObject());
    }

    /// <summary>
    ///Returns the object back to its original position when the pressure plate is not pressed.
    /// </summary>
    public void Deactivate()
    {
        StopCoroutine(MoveObject());
        StartCoroutine(ReturnObject());
    }

    /// <summary>
    /// Moves the object to its new position smoothly by using lerp.
    /// </summary>
    /// <returns>Returns null.</returns>
    private IEnumerator MoveObject()
    {
        yield return new WaitForSeconds(activationDelay);

        for (float liftRate = 0.0f; liftRate <= 1.1f; liftRate += 0.1f)
        {
            this.transform.position = Vector3.Lerp(originalPosition, newPosition, liftRate);
        }
    }

    /// <summary>
    /// Returns the object back to its original position smoothly by using lerp.
    /// </summary>
    /// <returns>Returns null.</returns>
    private IEnumerator ReturnObject()
    {
        yield return new WaitForSeconds(deactivationDelay);

        for (float dropRate = 0.0f; dropRate <= 1.1f; dropRate += 0.1f)
        {
            this.transform.position = Vector3.Lerp(newPosition, originalPosition, dropRate);
        }
    }
}
