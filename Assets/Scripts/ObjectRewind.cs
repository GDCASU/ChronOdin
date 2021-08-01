﻿/*
 * Records past references of an object at defined intervals.
 * Pressing [R] rewinds the object to the past references for a specified amount of time.
 * Immediately after rewinding, a cooldown is activated, denying another rewind cycle throughout its duration.
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
    [Tooltip("Time the object shall rewind for.")]
    private float rewindTime = 2f;

    [SerializeField]
    [Tooltip("The duration the object can't enter another rewind cycle immediately after rewinding.")]
    private float rewindCooldown = 2f;

    [SerializeField]
    [Tooltip("Time between saving object information for rewinding.")]
    private float timeBetweenSaves = 0.2f;

    private List<PastReference> references;  // saved references of the object
    private Rigidbody objectPhysics;  // for gathering object velocity and angular velocity
    private WaitForSeconds waitForCooldown;  // coroutine suspension time for cooldown

    private float maxReferences;  // max amount of references that can be saved
    private float timeSinceLastSave = 0f;  // time since the latest reference was saved

    private bool canInitiateRewind = true;  // Is the object moving with forward time and is the cooldown inactive?
    private bool isRewinding = false;  // Is the object rewinding?

    /// <summary>
    /// Initializes reference list, object physics and coroutine suspension for cooldown; calculates the max amount of references to be saved;
    /// records the first reference.
    /// </summary>
    private void Start()
    {
        references = new List<PastReference>();
        objectPhysics = transform.GetComponent<Rigidbody>();
        waitForCooldown = new WaitForSeconds(rewindCooldown);

        maxReferences =  Mathf.Round(rewindTime / timeBetweenSaves);
        Record();
    }

    /// <summary>
    /// Rewinds the object if the Player presses the rewind button and the object can enter a rewind cycle.
    /// </summary>
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R) && canInitiateRewind)
        {
            StartCoroutine(Rewind());
        }
    }

    /// <summary>
    /// Records references for the object if it is not rewinding.
    /// </summary>
    public void FixedUpdate()
    {
        // If the object is not rewinding, then update the time since the last reference save.
        // Once the time between saves has been reached, record a reference and reset the time since last reference.
        if (!isRewinding)
        {
            if (timeSinceLastSave < timeBetweenSaves)
            {
                timeSinceLastSave += Time.fixedDeltaTime;
            }
            else if (timeSinceLastSave >= timeBetweenSaves)
            {
                Record();
                timeSinceLastSave = 0f;
            }
        }
    }

    /// <summary>
    /// Rewinds the object to previous references whilst immediately activating the cooldown.
    /// </summary>
    private IEnumerator Rewind()
    {
        // Do not allow another Rewind coroutine to run, disable reference recording, and activate the cooldown.
        canInitiateRewind = false;
        isRewinding = true;
        objectPhysics.isKinematic = true;
        StartCoroutine(ActivateCooldown());

        // Declare variables.
        PastReference referenceToReach = references[references.Count - 1];
        Vector3 initialPosition, finalPosition;
        Quaternion initialRotation, finalRotation;
        bool isFromAReference = false;
        float timeToPreviousReference;
        float elapsedTimeRewinding = 0f;
        float elapsedTimeBetweenReferences = 0f;

        // Whilst there are saved references, lerp the object's position and rotation to its past references.
        while(references.Count > 0)
        {
            // Assign the closest past reference as the reference for the object to reach.
            referenceToReach = references[references.Count - 1];

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
            
            // Lerp the object to the reference to reach and update the elapsed time rewinding whilst the rewind time has not been reached.
            elapsedTimeBetweenReferences = 0f;
            while(elapsedTimeBetweenReferences < timeToPreviousReference)
            {
                // Check whether the object is done rewinding.
                if(elapsedTimeRewinding >= rewindTime)
                {
                    goto StopRewinding;
                }

                // Lerp.
                transform.position = Vector3.Lerp(initialPosition, finalPosition, elapsedTimeBetweenReferences / timeToPreviousReference);
                transform.rotation = Quaternion.Lerp(initialRotation, finalRotation, elapsedTimeBetweenReferences / timeToPreviousReference);

                // Update time.
                elapsedTimeBetweenReferences += Time.deltaTime;
                elapsedTimeRewinding += Time.deltaTime;
                yield return null;
            }

            // Snap the object position and rotation to the final position and rotation, and then remove a saved reference.
            transform.position = finalPosition;
            transform.rotation = finalRotation;
            references.RemoveAt(references.Count - 1);
        }

        // Grant the object the velocity and angular velocity of the closest reference, allow the object to move with forward time,
        // update the time the object is from the closest reference, and resume reference recording.
        StopRewinding:
        objectPhysics.velocity = referenceToReach.velocity;
        objectPhysics.angularVelocity = referenceToReach.angularVelocity;
        objectPhysics.isKinematic = false;
        timeSinceLastSave = timeBetweenSaves - elapsedTimeBetweenReferences;
        isRewinding = false;
    }

    /// <summary>
    /// Denies the object from being rewound for a time after the object begins rewinding.
    /// </summary>
    private IEnumerator ActivateCooldown()
    {
        yield return waitForCooldown;
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
