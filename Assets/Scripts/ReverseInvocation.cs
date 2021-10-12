using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ReverseInvocation : MonoBehaviour
{
    [Header("Button")]
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

    [Tooltip("The duration the Player shall be reversed for")]
    [SerializeField]
    private float reversePlayerTime = 10f;

    [Tooltip("The duration the Player can't be reversed after being reversed")]
    [SerializeField]
    private float reversePlayerCooldown = 10f;

    [Header("Transforms")]
    [Tooltip("The camera providing the Player vision")]
    [SerializeField]
    private Transform playerCamera;

    public static ReverseInvocation singleton; // singleton for detecting Player

    // Values for casting a ray to detect collisions.
    RaycastHit rayHit;
    private Vector3 startRayPosition, rayDirection;
    private int maxRayCasts = 2;
    private float rayPositionOffset = 0.000006f;

    // For suspending cooldown coroutines.
    private WaitForSeconds waitForObjectCooldown;
    private WaitForSeconds waitForPlayerCooldown;

    private bool canInitiateObjectReverse = true;  // Is the object reverse cooldown inactive?
    private bool canInitiatePlayerReverse = true;  // Is the Player reverse cooldown inactive?
    ObjectReverse objectToReverse = null;  // script attached to object that the Player desires to reverse
    PlayerReverse playerReversal = null;  // script attached to Player responsible for reversing the Player

    /// <summary>
    /// Assigns the singleton.
    /// </summary>
    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
    }

    /// <summary>
    /// Assigns coroutine suspension times and collects the PlayerReverse script.
    /// </summary>
    private void Start()
    {
        waitForObjectCooldown = new WaitForSeconds(reverseObjectCooldown);
        waitForPlayerCooldown = new WaitForSeconds(reversePlayerCooldown);

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
                    // Attempt to acquire the ObjectReverse script from the object the ray hit.
                    objectToReverse = rayHit.transform.GetComponent<ObjectReverse>();

                    // If the ray hits the Player, re-assign the starting position to be a bit away from the hit position
                    // in the previous ray's direction and continue to the next loop iteration.
                    if (rayHit.transform.gameObject.CompareTag("Player"))
                    {
                        startRayPosition = rayHit.point + (rayDirection.normalized * rayPositionOffset);
                        continue;
                    }
                    // If the ray hits an object with the ObjectReverse script, then reverse the object, activate the reverse object cooldown, and stop casting rays.
                    if (objectToReverse != null)
                    {
                        StartCoroutine(objectToReverse.Reverse());
                        StartCoroutine(ActivateObjectCooldown());
                        return;
                    }
                    // If the ray hits nothing, stop casting rays.
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
    /// Denies the Player from reversing an object throughout the reverse object cooldown.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ActivateObjectCooldown()
    {
        canInitiateObjectReverse = false;
        yield return waitForObjectCooldown;
        canInitiateObjectReverse = true;
    }

    /// <summary>
    /// Denies the Player from reversing themself throughout the reverse Player cooldown.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ActivatePlayerCooldown()
    {
        canInitiatePlayerReverse = false;
        yield return waitForPlayerCooldown;
        canInitiatePlayerReverse = true;
    }

    // Returns the duration an object shall reverse for.
    public float GetReverseObjectTime()
    {
        return reverseObjectTime;
    }
}
