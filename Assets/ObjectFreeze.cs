/*
 * Freezes the attached gameobject for a specified amount of time; cooldown activates right when the object freezes. After the object is unfrozen, it can not be frozen whilst the cooldown is active.
 * The freeze button is [F].
 * 
 * Author: Cristion Dominguez
 * Date: 28 July 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFreeze : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The time the object shall be frozen for.")]
    private float freezeTime = 5f;

    [SerializeField]
    [Tooltip("The duration the object can't be frozen after being frozen.")]
    private float freezeCooldown = 5f;

    // For suspending coroutines.
    private WaitForSeconds waitForFreezeTime;
    private WaitForSeconds waitForFreezeCooldown;

    private bool canInitiateFreeze = true;  // Is the object unfrozen and is the cooldown inactive?

    private Rigidbody objectPhysics;  // for collecting and saving velocity and angular velocity
    private Vector3 unfrozenVelocity, unfrozenAngularVelocity;  // saved before freezing object

    /// <summary>
    /// Gathers the rigidbody of the gameobject and assigns coroutine suspension times.
    /// </summary>
    private void Start()
    {
        objectPhysics = transform.GetComponent<Rigidbody>();

        waitForFreezeTime = new WaitForSeconds(freezeTime);
        waitForFreezeCooldown = new WaitForSeconds(freezeCooldown);
    }

    /// <summary>
    /// Freezes the gameobject if the Player presses F, the object is unfrozen, and the cooldown is inactive.
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && canInitiateFreeze)
        {
            StartCoroutine(FreezeObject());
        }
    }

    /// <summary>
    /// Freezes the gameobject and activates the cooldown, saving the object's velocity and angular velocity before doing so. After the freeze time is up, the velocity and angular
    /// velocity is returned to the object.
    /// </summary>
    private IEnumerator FreezeObject()
    {
        canInitiateFreeze = false;
        StartCoroutine(ActivateCooldown());

        unfrozenVelocity = objectPhysics.velocity;
        unfrozenAngularVelocity = objectPhysics.angularVelocity;
        objectPhysics.isKinematic = true;

        yield return waitForFreezeTime;
        
        objectPhysics.velocity = unfrozenVelocity;
        objectPhysics.angularVelocity = unfrozenAngularVelocity;
        objectPhysics.isKinematic = false;
    }

    /// <summary>
    /// Denies the object from being frozen throughout the freeze cooldown.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ActivateCooldown()
    {
        yield return waitForFreezeCooldown;
        canInitiateFreeze = true;
    }
}
