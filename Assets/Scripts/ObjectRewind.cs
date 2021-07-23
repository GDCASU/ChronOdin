/*
 * Records past references of an object at defined intervals after the object moves for the 1st time.
 * Pressing 'W' rewinds the object to its previous location.
 * Releasing 'W' allows the object to continue normal movement, inheriting velocity and angular velocity from the most recent reference.
 * 
 * Possible Improvements:
    * Method or script to calculate the proper velocity and angular velocity of the object when time is moved forward.
    * Record a reference of the object after cancelling rewind.
 * 
 * Author: Cristion Dominguez
 * Date: 22 July 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PastReference
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 velocity;
    public Vector3 angularVelocity;

    public PastReference(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
    {
        this.position = position;
        this.rotation = rotation;
        this.velocity = velocity;
        this.angularVelocity = angularVelocity;
    }
}

public class ObjectRewind : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Total time for rewind.")]
    private float rewindTime = 2f;

    [SerializeField]
    [Tooltip("Time between saving object information for rewind.")]
    private float timeBetweenSaves = 0.2f;

    private List<PastReference> references;
    private Rigidbody objectPhysics;

    private float maxPoints;
    private float timeSinceLastSave = 0f;

    private bool isRewinding = false;
    private bool isMoving = false;

    private void Start()
    {
        references = new List<PastReference>();
        objectPhysics = transform.GetComponent<Rigidbody>();

        maxPoints =  Mathf.Round(rewindTime / timeBetweenSaves);
        Record();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(Rewind());
        }
    }

    public void FixedUpdate()
    {
        if (isMoving && !isRewinding)
        {
            if(timeSinceLastSave < timeBetweenSaves)
            {
                timeSinceLastSave += Time.fixedDeltaTime;
            }
            else if(timeSinceLastSave >= timeBetweenSaves)
            {
                Record();
                timeSinceLastSave = 0f;
            }
        }
        else if (objectPhysics.velocity.magnitude >= 0.01 || objectPhysics.angularVelocity.magnitude >= 0.01)
        {
            isMoving = true;
        }
    }

    private IEnumerator Rewind()
    {
        isRewinding = true;
        objectPhysics.isKinematic = true;

        PastReference pointInTime;
        Vector3 initialPosition, finalPosition;
        Quaternion initialRotation, finalRotation;
        float elapsedTime = 0f;
        float timeToPreviousReference;

        Vector3 velocity = objectPhysics.velocity;
        Vector3 angularVelocity = objectPhysics.angularVelocity;

        bool isFromAReference = false;

        while(isRewinding && references.Count > 0)
        {
            pointInTime = references[references.Count - 1];
            velocity = pointInTime.velocity;
            angularVelocity = pointInTime.angularVelocity;

            initialPosition = transform.position;
            finalPosition = pointInTime.position;

            initialRotation = transform.rotation;
            finalRotation = pointInTime.rotation;

            if (!isFromAReference)
            {
                timeToPreviousReference = timeSinceLastSave;
                isFromAReference = true;
            }
            else
            {
                timeToPreviousReference = timeBetweenSaves;
            }
            
            elapsedTime = 0f;

            while(isRewinding && elapsedTime < timeToPreviousReference)
            {
                transform.position = Vector3.Lerp(initialPosition, finalPosition, elapsedTime / timeToPreviousReference);
                transform.rotation = Quaternion.Lerp(initialRotation, finalRotation, elapsedTime / timeToPreviousReference);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if(elapsedTime >= timeToPreviousReference)
            {
                transform.position = finalPosition;
                transform.rotation = finalRotation;

                references.RemoveAt(references.Count - 1);
            }
        }

        isRewinding = false;
        objectPhysics.isKinematic = false;

        objectPhysics.velocity = velocity;
        objectPhysics.angularVelocity = angularVelocity;

        timeSinceLastSave = timeBetweenSaves - elapsedTime;
    }

    private void Record()
    {
        if(references.Count > maxPoints)
        {
            references.RemoveAt(0);
        }

        references.Add(new PastReference(transform.position, transform.rotation, objectPhysics.velocity, objectPhysics.angularVelocity));
    }
}
