/*
 * Freezes objects for a specified amount of time.
 * The Player has the option to freeze a single object or all freezeable objects.
 * To freeze a single object, the Player must look at the object and press the freeze single object button whilst the corresponding cooldown is inactive.
 * To freeze the environment, the Player must press the freeze environment button whilst the corresponding cooldown is inactive.
 * 
 * Author: Cristion Dominguez
 * Date: 10 September 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FreezeInvocation : MonoBehaviour
{
    [Header("Button")]
    [Tooltip("The button to freeze a single object")]
    [SerializeField]
    private KeyCode freezeSingleObjectButton = KeyCode.F;

    [Tooltip("The button to freeze the entire environment")]
    [SerializeField]
    private KeyCode freezeEnvironmentButton = KeyCode.G;

    [Header("Time Values")]
    [Tooltip("The time the object shall be frozen for")]
    [SerializeField]
    private float freezeSingleTime = 5f;

    [Tooltip("The duration the object can't be frozen after being frozen")]
    [SerializeField]
    private float freezeSingleCooldown = 5f;

    [Tooltip("The duration the environment shall be frozen for")]
    [SerializeField]
    private float freezeEnvironmentTime = 10f;

    [Tooltip("The duration the environment can't be frozen after being frozen")]
    [SerializeField]
    private float freezeEnvironmentCooldown = 10f;

    [Header("Transforms")]
    [Tooltip("The camera providing the Player vision")]
    [SerializeField]
    private Transform playerCamera;

    // Values for casting a ray to detect collisions.
    RaycastHit rayHit;
    private Vector3 startRayPosition, rayDirection;
    private int maxRayCasts = 2;
    private float rayPositionOffset = 0.000006f;

    // For suspending cooldown coroutines.
    private WaitForSeconds waitForSingleCooldown;
    private WaitForSeconds waitForEnvironmentCooldown;

    private bool canInitiateSingleFreeze = true;  // Is the single freeze cooldown inactive?
    private bool canInitiateEnvironmentFreeze = true;  // Is the environment freeze cooldown inactive?
    ObjectFreeze objectToFreeze = null;  // script attached to object that the Player desires to freeze
    public static Action<float> freezeEveryObject;  // event container for freezing every freezeable object

    /// <summary>
    /// Assigns coroutine suspension times.
    /// </summary>
    private void Start()
    {
        waitForSingleCooldown = new WaitForSeconds(freezeSingleCooldown);
        waitForEnvironmentCooldown = new WaitForSeconds(freezeEnvironmentCooldown);
    }

    /// <summary>
    /// Freezes a single object or the environment depending on Player input and the ability to freeze a specific object(s).
    /// </summary>
    private void Update()
    {
        // If the Player presses the freeze single object button and the corresponding cooldown is inactive, attempt to freeze a single object.
        if (Input.GetKeyDown(freezeSingleObjectButton) && canInitiateSingleFreeze)
        {
            // Set the ray's starting position and direction.
            startRayPosition = playerCamera.position;
            rayDirection = playerCamera.TransformDirection(Vector3.forward);

            // Cast the ray until the ray does not hit the Player or maxRayCasts has been reached.
            for (int i = 0; i < maxRayCasts; i++)
            {
                if (Physics.Raycast(startRayPosition, rayDirection, out rayHit))
                {
                    // Attempt to acquire the ObjectFreeze script from the object the ray hit.
                    objectToFreeze = rayHit.transform.GetComponent<ObjectFreeze>();

                    // If the ray hits the Player, re-assign the starting position to be a bit away from the hit position
                    // in the previous ray's direction and continue to the next loop iteration.
                    if (rayHit.transform.gameObject.CompareTag("Player"))
                    {
                        startRayPosition = rayHit.point + (rayDirection.normalized * rayPositionOffset);
                        continue;
                    }
                    // If the ray hits an object with the ObjectFreeze script, then freeze the object, activate the freeze single object cooldown, and stop casting rays.
                    if (objectToFreeze != null)
                    {
                        objectToFreeze.StartFreeze(freezeSingleTime);
                        StartCoroutine(ActivateSingleCooldown());
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

        // If the Player presses the freeze environment button and the corresponding cooldown is inactive, attempt to freeze all freezeable objects.
        if (Input.GetKeyDown(freezeEnvironmentButton) && canInitiateEnvironmentFreeze)
        {
            // If there are freezeable objects existing in the scene, then freeze all of them and activate the freeze environment cooldown.
            MasterTime.singleton.UpdateTime(0);
            StartCoroutine(ActivateEnvironmentCooldown());
            if (freezeEveryObject != null) freezeEveryObject(freezeEnvironmentTime);
        }
    }

    /// <summary>
    /// Denies the Player from freezing an object throughout the freeze single cooldown.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ActivateSingleCooldown()
    {
        canInitiateSingleFreeze = false;
        yield return waitForSingleCooldown;
        canInitiateSingleFreeze = true;
    }

    /// <summary>
    /// Denies the Player from freezing the environment throughout the freeze environment cooldown.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ActivateEnvironmentCooldown()
    {
        canInitiateEnvironmentFreeze = false;
        yield return waitForEnvironmentCooldown;
        MasterTime.singleton.UpdateTime(1);
        canInitiateEnvironmentFreeze = true;
    }
}
