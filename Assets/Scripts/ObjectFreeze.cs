/*
 * Freezes the attached gameobject for a specified amount of time. After the object is unfrozen, it can not be frozen for a specified amount of time.
 * Right now, the freeze button is [F].
 * 
 * Author: Cristion Dominguez
 * Date: 28 July 2021
 * 
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
    [Tooltip("The duration the object can't be frozen after being unfrozen.")]
    private float freezeCooldown = 5f;

    private bool canFreeze = true;  // Is the object unfrozen and is the cooldown inactive?

    private Rigidbody objectPhysics;  // for collecting and saving velocity and angular velocity
    private Vector3 unfrozenVelocity, unfrozenAngularVelocity;  // saved before freezing object

    /// <summary>
    /// Gathers the rigidbody of the gameobject.
    /// </summary>
    private void Start()
    {
        objectPhysics = transform.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Freezes the gameobject if the Player presses F, the object is unfrozen, and the cooldown is not active.
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && canFreeze)
        {
            StartCoroutine(freezeObject());
        }
    }

    /// <summary>
    /// Freezes the gameobject, saving its velocity and angular velocity before doing so. After the freeze time is up, the velocity and angular velocity is returned to
    /// the object, and the cooldown becomes active.
    /// </summary>
    private IEnumerator freezeObject()
    {
        canFreeze = false;

        unfrozenVelocity = objectPhysics.velocity;
        unfrozenAngularVelocity = objectPhysics.angularVelocity;
        objectPhysics.isKinematic = true;

        yield return new WaitForSeconds(freezeTime);
        
        objectPhysics.velocity = unfrozenVelocity;
        objectPhysics.angularVelocity = unfrozenAngularVelocity;
        objectPhysics.isKinematic = false;

        StartCoroutine(runCooldown());
    }

    /// <summary>
    /// Denies the object from being frozen throughout the freeze cooldown.
    /// </summary>
    /// <returns></returns>
    private IEnumerator runCooldown()
    {
        yield return new WaitForSeconds(freezeCooldown);
        canFreeze = true;
    }
}
