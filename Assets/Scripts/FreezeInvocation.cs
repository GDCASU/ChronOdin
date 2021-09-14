/*
 * Freezes objects for a specified amount of time.
 * The Player has the option to freeze a single object or all freezeable objects.
 * To freeze a single object, the Player must look at the object
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
    private KeyCode freezeSingleButton = KeyCode.F;

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

    // For suspending cooldown coroutines.
    private WaitForSeconds waitForSingleCooldown;
    private WaitForSeconds waitForEnvironmentCooldown;

    private bool canInitiateSingleFreeze = true;  // Is the object unfrozen and is the cooldown inactive?
    private bool canInitiateEnvironmentFreeze = true;

    ObjectFreeze objectToFreeze = null;

    public static Action<float> freezeEveryObject;

    /// <summary>
    /// Gathers the rigidbody of the gameobject and assigns coroutine suspension times.
    /// </summary>
    private void Start()
    {
        waitForSingleCooldown = new WaitForSeconds(freezeSingleCooldown);
        waitForEnvironmentCooldown = new WaitForSeconds(freezeEnvironmentCooldown);
    }

    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(freezeSingleButton) && canInitiateSingleFreeze)
        {
            // Set the ray's starting position and direction.
            Vector3 startRayPosition = playerCamera.position;
            Vector3 rayDirection = playerCamera.TransformDirection(Vector3.forward);
            int maxRayCasts = 2;
            RaycastHit rayHit;

            // Cast the ray until the ray does not hit the Player or maxRayCasts has been reached.
            for (int i = 0; i < maxRayCasts; i++)
            {
                if (Physics.Raycast(startRayPosition, rayDirection, out rayHit))
                {
                    objectToFreeze = rayHit.transform.GetComponent<ObjectFreeze>();

                    if (objectToFreeze != null)
                    {
                        canInitiateSingleFreeze = false;
                        StartCoroutine(objectToFreeze.FreezeObject(freezeSingleTime));
                        StartCoroutine(ActivateSingleCooldown());
                    }
                    // If the ray hits nothing, stop casting rays.
                    // FOR TESTING PURPOSES, comment this "else" block out.
                    else
                    {
                        return;
                    }
                }
            }
            
        }

        if (Input.GetKeyDown(freezeEnvironmentButton) && canInitiateEnvironmentFreeze)
        {
            if (freezeEveryObject != null)
            {
                Debug.Log("Hi");
                canInitiateEnvironmentFreeze = false;
                freezeEveryObject(freezeEnvironmentTime);
                StartCoroutine(ActivateEnvironmentCooldown());
            }
        }
    }

    /// <summary>
    /// Denies the Player from freezing an object throughout the freeze single cooldown.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ActivateSingleCooldown()
    {
        yield return waitForSingleCooldown;
        canInitiateSingleFreeze = true;
    }

    /// <summary>
    /// Denies the Player from freezing the environment throughout the freeze environment cooldown.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ActivateEnvironmentCooldown()
    {
        yield return waitForEnvironmentCooldown;
        canInitiateEnvironmentFreeze = true;
    }
}
