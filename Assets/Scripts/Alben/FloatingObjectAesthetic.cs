using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Set an object to float while rotating and moving in a loop.
/// </summary>
public class FloatingObjectAesthetic : MonoBehaviour
{
    [Tooltip("Number of seconds before it changes its velocity direction")] 
    public float directionTime = 0.0f;
    [Tooltip("Multiply the directionTime to change how far the object can move back and forth")] 
    public int directionWaitMultiplier = 2;
    [Tooltip("Speed that will gradually get faster in the x direction")] 
    public float startSpeedX = 0.0f;
    [Tooltip("Speed that will gradually get faster in the y direction")] 
    public float startSpeedY = 0.0f;
    [Tooltip("Speed that will gradually get faster in the z direction")] 
    public float startSpeedZ = 0.0f;
    [Tooltip("Rotation speed using x")] 
    public float maxRotateSpeedX = 0.0f;
    [Tooltip("Rotation speed using y")] 
    public float maxRotateSpeedY = 0.0f;
    [Tooltip("Rotation speed using z")] 
    public float maxRotateSpeedZ = 0.0f;

    Rigidbody rb;
    Vector3 acceleratedMovement;
    // bool readyToChangeDir;
    float rotateX, rotateY, rotateZ;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        acceleratedMovement = new Vector3(startSpeedX, startSpeedY, startSpeedZ);
        // readyToChangeDir = false;

        /*if (maxRotateSpeedX > 0)
            rotateX = Random.Range(0, maxRotateSpeedX);
        else
            rotateX = Random.Range(maxRotateSpeedX, 0);

        if (maxRotateSpeedY > 0)
            rotateY = Random.Range(0, maxRotateSpeedY);
        else
            rotateY = Random.Range(maxRotateSpeedY, 0);

        if (maxRotateSpeedZ > 0)
            rotateZ = Random.Range(0, maxRotateSpeedZ);
        else
            rotateZ = Random.Range(maxRotateSpeedZ, 0);*/

        rotateX = maxRotateSpeedX;
        rotateY = maxRotateSpeedY;
        rotateZ = maxRotateSpeedZ;

        StartCoroutine(switchDirections(directionTime));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Rotation
        transform.Rotate(rotateX, rotateY, rotateZ);

        // Movement
        rb.AddForce(acceleratedMovement, ForceMode.Acceleration);
        /*if (readyToChangeDir && (Mathf.Approximately(rb.velocity.x, 0) || Mathf.Approximately(rb.velocity.y, 0) || Mathf.Approximately(rb.velocity.z, 0)))
        {
            readyToChangeDir = false;
            StartCoroutine(switchDirections(this.directionTime * directionWaitMultiplier));
        }*/
    }

    /// <summary>
    /// Object waits a number of seconds before reversing its direction.
    /// </summary>
    /// <param name="directionTime">Number of seconds before it changes direction.</param>
    /// <returns>Waits for a number of seconds before changing direction.</returns>
    private IEnumerator switchDirections(float directionTime)
    {
        yield return new WaitForSeconds(directionTime);
        // readyToChangeDir = true;
        acceleratedMovement = acceleratedMovement * -1;
        StartCoroutine(switchDirections(directionTime * directionWaitMultiplier));
    }
}
