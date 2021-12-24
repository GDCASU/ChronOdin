/*
 * Records past references of an attached gameobject with a Rigidbody at defined intervals.
 * When Reverse() is called, the object reverses to the past references for a specified amount of time.
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

public class ObjectReverse : ComplexReverse
{
    [SerializeField]
    [Tooltip("Time between saving object information for reversing.")]
    private float timeBetweenSaves = 0.2f;

    private float timeBetweenSavesWhilstSlowed;  // time between saving object information when slowed

    private float totalReverseTime;  // the duration the object shall reverse for

    private List<PastReference> references;  // saved references of the object
    private Rigidbody objectPhysics;  // for gathering object velocity and angular velocity

    private float maxReferences;  // max amount of references that can be saved
    private float timeSinceLastSave = 0f;  // time since the latest reference was saved

    private bool isReversing = false;  // Is the object reversing?

    /// <summary>
    /// Subscribes a method to an event; initializes reference list and object physics; calculates the max amount of references to be saved; records the first reference.
    /// </summary>
    private void Start()
    {
        effectHub.broadcastTransition += ReceiveTransition;

        references = new List<PastReference>();
        objectPhysics = transform.GetComponent<Rigidbody>();

        totalReverseTime = PlayerController.singleton.transform.GetComponent<ReverseInvocation>().GetReverseObjectTime();
        maxReferences =  Mathf.Round(totalReverseTime / timeBetweenSaves);
        Record(TimeEffect.None);
    }

    /// <summary>
    /// Records references for the object if it is not reversing.
    /// </summary>
    private void FixedUpdate()
    {
        // If the object is not reversing, then update the time since the last reference save.
        // Once the time between saves has been reached, record a reference and reset the time since last reference.
        if (!isReversing)
        {
            // For an object moving normally with time.
            if (effectHub.CurrentEffect == TimeEffect.None)
            {
                if (timeSinceLastSave < timeBetweenSaves)
                {
                    timeSinceLastSave += Time.fixedDeltaTime;
                }
                else if (timeSinceLastSave >= timeBetweenSaves)
                {
                    Record(TimeEffect.None);
                    timeSinceLastSave = 0f;
                }
            }
            // For an object being slowed down in time.
            else if (effectHub.CurrentEffect == TimeEffect.Slow)
            {
                if (timeSinceLastSave < timeBetweenSavesWhilstSlowed)
                {
                    timeSinceLastSave += Time.fixedDeltaTime;
                }
                else if (timeSinceLastSave >= (timeBetweenSaves * (1 / effectHub.CurrentTimescale)))
                {
                    Record(TimeEffect.Slow);
                    timeSinceLastSave = 0f;
                }
            }
        }
    }

    /// <summary>
    /// Increases the timeSinceLastSave and timeBetweenSavesWhilstSlowed values by the timescale right before the object experiences the slow effect; OR
    /// decreases the timeSinceLastSave value by the timescale right after the object finishes the slow effect.
    /// </summary>
    private void ReceiveTransition()
    {
        if (effectHub.CurrentEffect == TimeEffect.Slow)
        {
            timeSinceLastSave = timeSinceLastSave / effectHub.CurrentTimescale;
            timeBetweenSavesWhilstSlowed = timeBetweenSaves / effectHub.CurrentTimescale;
        }
        if (effectHub.PreviousEffect == TimeEffect.Slow)
        {
            timeSinceLastSave = timeSinceLastSave * effectHub.PreviousTimescale;
        }
    }

    /// <summary>
    /// Reverses the object to previous references for total reverse time assigned at the Start() function.
    /// If the effect hub communicates that a new effect was introduced, then the object is unreversed and the next effect is transitioned to.
    /// </summary>
    /// <param name="reverseTime"> not utilized </param>
    public override void Reverse(float reverseTime) => StartCoroutine(ReverseObject(reverseTime));
    private IEnumerator ReverseObject(float reverseTime)
    {
        // Disable reference recording and object collisions.
        isReversing = true;
        objectPhysics.isKinematic = true;

        // Declare variables.
        PastReference referenceToReach = references[references.Count - 1];
        Vector3 initialPosition, finalPosition;
        Quaternion initialRotation, finalRotation;
        bool isFromAReference = false;
        float timeToPreviousReference;
        float elapsedTimeRewinding = 0f;
        float elapsedTimeBetweenReferences = 0f;

        // Whilst there are saved references and a new time effect is not introduced, lerp the object's position and rotation to its past references.
        while (references.Count > 0 && effectHub.IntroducingNewEffect == false)
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
            
            // Lerp the object to the reference to reach and update the elapsed time reversing whilst the rewind time has not been reached.
            elapsedTimeBetweenReferences = 0f;
            while(elapsedTimeBetweenReferences < timeToPreviousReference)
            {
                // Check whether the object is done reversing or a new time effect was introduced.
                if (elapsedTimeRewinding >= totalReverseTime || effectHub.IntroducingNewEffect == true)
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

        // Transition to the next time effect and indicate that reversing is finished.
        effectHub.TransitionToNextEffect();
        isReversing = false;
    }

    /// <summary>
    /// Saves a new reference of the object. If the reference limit has been met, the oldest reference is removed to allow the newest reference in the list.
    /// If the time effect is none, then the position, rotation, velocity, and angular velocity is saved as is.
    /// If the time effect is slow, then the position and rotation are saved as is whilst the velocity and angular velocity are increased by the current timescale.
    /// </summary>
    /// <param name="effect"> time effect the gameobject is currently experiencing </param>
    private void Record(TimeEffect effect)
    {
        if(references.Count > maxReferences)
        {
            references.RemoveAt(0);
        }

        if (effect == TimeEffect.None)
        {
            references.Add(new PastReference(transform.position, transform.rotation, objectPhysics.velocity, objectPhysics.angularVelocity));

        }
        else if (effect == TimeEffect.Slow)
        {
            references.Add(new PastReference(transform.position, transform.rotation, objectPhysics.velocity / effectHub.CurrentTimescale, objectPhysics.angularVelocity / effectHub.CurrentTimescale));
        }
    }
}
