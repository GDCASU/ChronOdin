/*
 * Freezes the attached gameobject for a specified amount of time; cooldown activates right when the object freezes. After the object is unfrozen, it can not be frozen whilst the cooldown is active.
 * 
 * Author: Cristion Dominguez
 * Date: 28 July 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFreeze : MonoBehaviour
{
    private Rigidbody objectPhysics;  // for collecting and saving velocity and angular velocity
    private Vector3 unfrozenVelocity, unfrozenAngularVelocity;  // saved before freezing object

    private FreezeInvocation playerFreeze;

    /// <summary>
    /// Gathers the rigidbody of the gameobject and assigns coroutine suspension times.
    /// </summary>
    private void Start()
    {
        FreezeInvocation.freezeEveryObject += StartFreeze;

        objectPhysics = transform.GetComponent<Rigidbody>();
    }

    private void StartFreeze(float freezeTime)
    {
        StartCoroutine(FreezeObject(freezeTime));
    }

    /// <summary>
    /// Freezes the gameobject and activates the cooldown, saving the object's velocity and angular velocity before doing so. After the freeze time is up, the velocity and angular
    /// velocity is returned to the object.
    /// </summary>
    public IEnumerator FreezeObject(float freezeTime)
    {
        unfrozenVelocity = objectPhysics.velocity;
        unfrozenAngularVelocity = objectPhysics.angularVelocity;
        objectPhysics.isKinematic = true;

        yield return new WaitForSeconds(freezeTime);
        
        objectPhysics.velocity = unfrozenVelocity;
        objectPhysics.angularVelocity = unfrozenAngularVelocity;
        objectPhysics.isKinematic = false;
    }
}
