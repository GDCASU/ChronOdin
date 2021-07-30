/*
 * Records past references of an object at defined intervals after the object moves for the 1st time.
 * Holding [R] rewinds the object to its previous location.
 * Releasing [R] allows the object to move with forward time, inheriting velocity and angular velocity from the most recent reference.
 * A cooldown is then imposed on the rewinding mechanic for the object, which is determined by how long the Player rewound the object for.
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

/// <summary>
/// Information about an object at a certain time.
/// </summary>
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
    [Tooltip("Total time the object can be rewound.")]
    private float maxRewindTime = 2f;

    [SerializeField]
    [Tooltip("Time between saving object information for rewinding.")]
    private float timeBetweenSaves = 0.2f;

    private List<PastReference> references;  // saved references of the object
    private Rigidbody objectPhysics;  // for gathering object velocity and angular velocity

    private float maxReferences;  // max amount of references that can be saved
    private float timeSinceLastSave = 0f;  // time since the latest reference was saved
    private float rewindCooldown = 0f;  //  cooldown after the rewind finishes

    private bool canInitiateRewind = true;  // Is the object moving with forward time and is the cooldown inactive?
    private bool isRewinding = false;  // Is the object rewinding?
    private bool hasMoved = false;  // Has the object moved?    

    /// <summary>
    /// Initializes reference list and object physics, calculates the max amount of references to be saved, and records the first reference.
    /// </summary>
    private void Start()
    {
        references = new List<PastReference>();
        objectPhysics = transform.GetComponent<Rigidbody>();

        maxReferences =  Mathf.Round(maxRewindTime / timeBetweenSaves);
        Record();
    }

    /// <summary>
    /// Rewinds the object if the Player hold down the rewind button and the object can start rewinding. Otherwise, ceases the object from rewinding.
    /// </summary>
    private void Update()
    {
        if(Input.GetKey(KeyCode.R) && canInitiateRewind)
        {
            isRewinding = true;
            StartCoroutine(Rewind());
        }
        else if(Input.GetKeyUp(KeyCode.R))
        {
            isRewinding = false;
        }
    }

    /// <summary>
    /// Records references for the object after it has moved and is not rewinding.
    /// </summary>
    public void FixedUpdate()
    {
        // If the object has moved and is not rewinding, then update the time since the last reference save and once it surpasses the time between saves, record a reference and reset the time since last reference.
        if (hasMoved && !isRewinding)
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
        // Commence recording references when the object's velocity and angular velocity reach a certain magnitude.
        else if (objectPhysics.velocity.magnitude >= 0.01 || objectPhysics.angularVelocity.magnitude >= 0.01)
        {
            hasMoved = true;
        }
    }

    /// <summary>
    /// Rewinds the object to previous references.
    /// </summary>
    private IEnumerator Rewind()
    {
        // Do not allow another Rewind coroutine to run and reset the rewind cooldown.
        canInitiateRewind = false;
        rewindCooldown = 0f;

        // Declare variables.
        PastReference referenceToReach;
        Vector3 initialPosition, finalPosition;
        Quaternion initialRotation, finalRotation;
        float elapsedTime = 0f;
        float timeToPreviousReference;
        bool isFromAReference = false;

        // Save the object velocity and angular velocity before freezing the object.
        Vector3 velocity = objectPhysics.velocity;
        Vector3 angularVelocity = objectPhysics.angularVelocity;
        objectPhysics.isKinematic = true;

        // Whilst [R] is being held down and there are saved references, lerp the object's position and rotation to its past references.
        while(isRewinding && references.Count > 0)
        {
            // Assign the closest past reference as the reference for the object to reach, and save the reference's velocity and angular velocity.
            referenceToReach = references[references.Count - 1];
            velocity = referenceToReach.velocity;
            angularVelocity = referenceToReach.angularVelocity;

            // Assign positions and rotations for lerping.
            initialPosition = transform.position;
            finalPosition = referenceToReach.position;
            initialRotation = transform.rotation;
            finalRotation = referenceToReach.rotation;

            // If the object is rewinding from a reference it met, then the total time for lerping is the time between reference saves.
            // Otherwise, the total time for lerping is the time after the last reference was saved.
            if (isFromAReference)
            {
                timeToPreviousReference = timeBetweenSaves;
            }
            else
            {
                timeToPreviousReference = timeSinceLastSave;
                isFromAReference = true;
            }
            
            // Lerp the object to the reference to reach whilst the Player is holding the rewind button, updating the rewind cooldown.
            elapsedTime = 0f;
            while(isRewinding && elapsedTime < timeToPreviousReference)
            {
                transform.position = Vector3.Lerp(initialPosition, finalPosition, elapsedTime / timeToPreviousReference);
                transform.rotation = Quaternion.Lerp(initialRotation, finalRotation, elapsedTime / timeToPreviousReference);

                elapsedTime += Time.deltaTime;
                rewindCooldown += Time.deltaTime;
                yield return null;
            }

            // If the time lerping surpasses the time to the previous reference, then snap the object to the reference's position and rotation as well as removing the reference from the save list.
            if(elapsedTime >= timeToPreviousReference)
            {
                transform.position = finalPosition;
                transform.rotation = finalRotation;

                references.RemoveAt(references.Count - 1);
            }
        }

        isRewinding = false;
        
        // Grant the object the velocity and angular velocity of the closest reference, and allow it to move with forward time.
        objectPhysics.velocity = velocity;
        objectPhysics.angularVelocity = angularVelocity;
        objectPhysics.isKinematic = false;

        // Update the time since last reference save and activate the cooldown.
        timeSinceLastSave = timeBetweenSaves - elapsedTime;
        StartCoroutine(RunCooldown());
    }

    /// <summary>
    /// Denies the object from being rewound for the time it has been rewinding after the object has regained forward movement.
    /// </summary>
    private IEnumerator RunCooldown()
    {
        yield return new WaitForSeconds(rewindCooldown);
        canInitiateRewind = true;

    }

    /// <summary>
    /// Saves a new reference of the object. If the reference limit has been met, the oldest reference is removed to allow the newest reference in the list.
    /// </summary>
    private void Record()
    {
        if(references.Count > maxReferences)
        {
            references.RemoveAt(0);
        }

        references.Add(new PastReference(transform.position, transform.rotation, objectPhysics.velocity, objectPhysics.angularVelocity));
    }
}
