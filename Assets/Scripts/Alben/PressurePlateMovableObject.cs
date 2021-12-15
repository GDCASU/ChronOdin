using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object that is moved when the pressure plate is pressed or down when the plate
/// is not pressed.
/// Author: Alben Trang
/// </summary>
public class PressurePlateMovableObject: SimpleTimeManipulation, LinkedToPressurePlate
{
    [Tooltip("The objects new position")]
    public Vector3 newPosition;

    [Tooltip("Speed at which the object moves")]
    public float speed = 3.0f;

    [Header("Activation Delays")]
    [Tooltip("Set how many seconds before the object is activated")]
    public float activationDelay = .5f;
    [Tooltip("Set how many seconds before the object is deactivated (Tip: if the pressure plate is pressed briefly, set this higher than activationDelay for better effect)")]
    public float deactivationDelay = 1f;

    //private float timeScale;
    private Vector3 originalPosition;

    /// <summary>
    /// Start at frame one to store the object's original position and it's new position.
    /// </summary>
    protected override void Start()
    {
        timeScale = MasterTime.singleton.timeScale;
        originalPosition = transform.position;
        newPosition = originalPosition + newPosition;
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
        Vector3 startingPosition = transform.position;
        float step = 0;
        while (step < 1)
        {
            step += speed * timeScale * Time.fixedDeltaTime;
            transform.position = Vector3.Lerp(startingPosition, newPosition, step);
            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// Returns the object back to its original position smoothly by using lerp.
    /// </summary>
    /// <returns>Returns null.</returns>
    private IEnumerator ReturnObject()
    {
        yield return new WaitForSeconds(deactivationDelay);
        Vector3 startingPosition = transform.position;
        float step = 0;
        while (step < 1)
        {
            step += speed * timeScale * Time.fixedDeltaTime;
            transform.position = Vector3.Lerp(startingPosition, originalPosition, step);
            yield return new WaitForFixedUpdate();
        }
    }
}
