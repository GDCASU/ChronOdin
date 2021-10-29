/*
 * Slows all slowable objects for a specified amount of time.
 * To freeze the environment, the Player must press the slow environment button whilst the corresponding cooldown is inactive.
 * 
 * Author: Cristion Dominguez
 * Date: 17 September 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SlowInvocation : MonoBehaviour
{
    [Header("Button")]
    [Tooltip("The button to slow the entire environment")]
    [SerializeField]
    private KeyCode slowEnvironmentButton = KeyCode.V;

    [Header("Time Values")]
    [Tooltip("The time the object shall be slowed for")]
    [SerializeField]
    private float slowEnvironmentTime = 5f;

    [Tooltip("The duration the environment can't be slowed after being slowed")]
    [SerializeField]
    private float slowEnvironmentCooldown = 10f;

    [Header("Intensity")]
    [Tooltip("How slow the object shall be")]
    [SerializeField]
    private float slowDownFactor = 0.5f;

    // For suspending cooldown coroutines.
    private WaitForSeconds waitForEnvironmentCooldown;

    private bool canInitiateEnvironmentSlow = true;  // Is the environment slow cooldown inactive?
    public static Action<float, float> slowEveryObject;  // event container for slowing every slowable object

    /// <summary>
    /// Assigns coroutine suspension times.
    /// </summary>
    private void Start()
    {
        waitForEnvironmentCooldown = new WaitForSeconds(slowEnvironmentCooldown);
    }

    /// <summary>
    /// Slows the environment when Player presses the slow environment button and the slow cooldown is inactive.
    /// </summary>
    private void Update()
    {
        // If the Player presses the slow environment button and the corresponding cooldown is inactive, attempt to slow all slowable objects.
        if (Input.GetKeyDown(slowEnvironmentButton) && canInitiateEnvironmentSlow)
        {
            // If there are slowable objects existing in the scene, then slow all of them and activate the slow environment cooldown.
            if (slowEveryObject != null)
            {
                MasterTime.singleton.UpdateTime(5);
                slowEveryObject(slowEnvironmentTime, slowDownFactor);
                StartCoroutine(ActivateEnvironmentCooldown());
            }
        }
    }

    /// <summary>
    /// Denies the Player from slowing the environment throughout the slow environment cooldown.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ActivateEnvironmentCooldown()
    {
        canInitiateEnvironmentSlow = false;
        yield return waitForEnvironmentCooldown;
        MasterTime.singleton.UpdateTime(1);
        canInitiateEnvironmentSlow = true;
    }
}
