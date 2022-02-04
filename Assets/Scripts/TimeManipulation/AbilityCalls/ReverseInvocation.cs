/*
 * Reverses an object or the Player for a specified amount of time.
 * To reverse a single object, the Player must look at the object and press the reverse object button whilst the corresponding cooldown is inactive.
 * To reverse the Player, the Player must press the reverse player button whilst the corresponding cooldown is inactive.
 * 
 * Author: Cristion Dominguez
 * Date: 21 November 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ReverseInvocation : MonoBehaviour
{
    [Header("Buttons")]
    [Tooltip("The button to reverse a single object")]
    [SerializeField]
    private KeyCode reverseObjectButton = KeyCode.R;

    [Tooltip("The button to reverse the Player")]
    [SerializeField]
    private KeyCode reversePlayerButton = KeyCode.E;

    [Header("Time Values")]
    [Tooltip("The duration the object shall be reverse for")]
    [SerializeField]
    private float reverseObjectTime = 5f;

    [Tooltip("The duration the object can't be reversed after being reversed")]
    [SerializeField]
    private float reverseObjectCooldown = 5f;

    /*
    [Tooltip("The duration the Player shall be reversed for")]
    [SerializeField]
    private float reversePlayerTime = 10f;
    */

    [Tooltip("The duration the Player can't be reversed after being reversed")]
    [SerializeField]
    private float reversePlayerCooldown = 10f;

    [Header("Transforms")]
    [Tooltip("The camera providing the Player vision")]
    [SerializeField]
    private Transform playerCamera;

    // Values for casting a ray to detect collisions.
    RaycastHit rayHit;
    private Vector3 startRayPosition, rayDirection;
    private int maxRayCasts = 2;
    private float rayPositionOffset = 0.000006f;

    // The remaining times for active reverse sub-abilities and cooldowns
    [HideInInspector]
    public float RemainingObjectActiveTime { get; private set; }
    [HideInInspector]
    public float RemainingObjectCooldown { get; private set; }
    [HideInInspector]
    public float RemainingPlayerCooldown { get; private set; }

    private bool canInitiateObjectReverse = true;  // Is the object reverse cooldown inactive?
    private bool canInitiatePlayerReverse = true;  // Is the Player reverse cooldown inactive?
    SimpleTimeManipulation simpleObject = null;  // object with a simple reverse mechanism
    ComplexTimeHub complexObject = null;  // object with a complex reverse mechanism
    PlayerReverse playerReversal = null;  // script attached to Player responsible for reversing the Player

    /// <summary>
    /// Collects the PlayerReverse script.
    /// </summary>
    private void Start()
    {
        playerReversal = transform.GetComponent<PlayerReverse>();
    }

    /// <summary>
    /// Reverses a single object or the Player depending on Player input and the ability to reverse.
    /// </summary>
    private void Update()
    {
        // If the Player presses the reverse object button and the corresponding cooldown is inactive, attempt to reverse a single object.
        if (Input.GetKeyDown(reverseObjectButton) && canInitiateObjectReverse)
        {
            // Set the ray's starting position and direction.
            startRayPosition = playerCamera.position;
            rayDirection = playerCamera.TransformDirection(Vector3.forward);

            // Cast the ray until the ray does not hit the Player or maxRayCasts has been reached.
            for (int i = 0; i < maxRayCasts; i++)
            {
                if (Physics.Raycast(startRayPosition, rayDirection, out rayHit))
                {
                    // If the ray hits the Player, re-assign the starting position to be a bit away from the hit position
                    // in the previous ray's direction and continue to the next loop iteration.
                    if (rayHit.transform.gameObject.CompareTag("Player"))
                    {
                        startRayPosition = rayHit.point + (rayDirection.normalized * rayPositionOffset);
                        continue;
                    }

                    // If the ray does not hit the Player, attempt to detect an object that can be reversed.
                    simpleObject = rayHit.transform.GetComponent<SimpleTimeManipulation>();
                    complexObject = rayHit.transform.GetComponent<ComplexTimeHub>();

                    // If the ray hits an object that can be reversed, then reverse the object, activate the reverse object cooldown, and stop casting rays.
                    if (simpleObject != null)
                    {
                        simpleObject.UpdateTimeScale(-1f);
                        StartCoroutine(ActivateObjectCooldown());
                        StartCoroutine(CountdownObjectReverse(simpleObject));
                        return;
                        
                    }
                    else if (complexObject != null)
                    {
                        // If the object does not possess a reverse script, then do not activate cooldown.
                        if (complexObject.transform.GetComponent<ComplexReverse>() == null)
                        {
                            return;
                        }

                        complexObject.AffectObject(TimeEffect.Reverse, reverseObjectTime, -1f);
                        StartCoroutine(ActivateObjectCooldown());
                        return;
                    }
                    // If the ray hits an object that can't be reversed, then stop casting rays.
                    // FOR TESTING PURPOSES, comment this "else" block out.
                    else
                    {
                        return;
                    }
                }
            }

            // FOR TESTING PURPOSES, remove the comments for the block below.
            /*
             if (rayHit.transform != null)
                Debug.Log(rayHit.transform.name);
             else
                Debug.Log("N/A");
            */
        }

        // If the Player presses the reverse Player button and the corresponding cooldown is inactive, attempt to reverse the Player.
        if (Input.GetKeyDown(reversePlayerButton) && canInitiatePlayerReverse)
        {
            playerReversal.CallReverse();
            StartCoroutine(ActivatePlayerCooldown());
        }
    }

    /// <summary>
    /// Updates the simple object's timescale to the default value after the object active time passes.
    /// Notifies the SimpleTargetAbilityTracker to forgot the saved reversing object.
    /// </summary>
    /// <param name="simpleObject"> object with a simple reverse mechanism </param>
    /// <returns></returns>
    private IEnumerator CountdownObjectReverse(SimpleTimeManipulation simpleObject)
    {
        RemainingObjectActiveTime = reverseObjectTime;
        while (RemainingObjectActiveTime > 0)
        {
            RemainingObjectActiveTime -= Time.deltaTime;
            yield return null;
        }

        if (simpleObject != null)
        {
            SimpleTargetAbilityTracker.singleton.ResetReversingObject();
            simpleObject.UpdateTimeScale(1f);
        }
    }

    /// <summary>
    /// Denies the Player from reversing an object throughout the reverse object cooldown.
    /// </summary>
    private IEnumerator ActivateObjectCooldown()
    {
        canInitiateObjectReverse = false;

        RemainingObjectCooldown = reverseObjectCooldown;
        while (RemainingObjectCooldown > 0)
        {
            RemainingObjectCooldown -= Time.deltaTime;
            yield return null;
        }

        canInitiateObjectReverse = true;
    }

    /// <summary>
    /// Denies the Player from reversing themself throughout the reverse Player cooldown.
    /// </summary>
    private IEnumerator ActivatePlayerCooldown()
    {
        canInitiatePlayerReverse = false;

        RemainingPlayerCooldown = reversePlayerCooldown;
        while (RemainingPlayerCooldown > 0)
        {
            RemainingPlayerCooldown -= Time.deltaTime;
            yield return null;
        }

        canInitiatePlayerReverse = true;
    }

    /// <summary>
    /// Returns the duration an object shall reverse for.
    /// </summary>
    /// <returns></returns>
    public float GetReverseObjectTime()
    {
        return reverseObjectTime;
    }
}
