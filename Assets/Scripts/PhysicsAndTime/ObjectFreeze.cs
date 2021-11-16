/*
 * Freezes the attached gameobject for a specified amount of time.
 * 
 * Author: Cristion Dominguez
 * Date: 15 September 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFreeze : ComplexFreeze
{
    private Rigidbody objectPhysics;  // for collecting and saving velocity, angular velocity, and rigidbody contraints

    // Save before freezing gameobject.
    private Vector3 unfrozenVelocity, unfrozenAngularVelocity;
    private RigidbodyConstraints previousContraints;

    /// <summary>
    /// Collects the attached object's rigidbody and subscribes the StartFreeze method to the Player's freeze environment ability.
    /// </summary>
    private void Start()
    {
        objectPhysics = transform.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Commences the coroutine for freezing the gameobject.
    /// Necessary for freeze environment event to function correctly.
    /// </summary>
    /// <param name="freezeTime"> time to freeze object </param>
    public override void Freeze(float freezeTime)
    {
        StartCoroutine(FreezeObject(freezeTime));
    }

    /// <summary>
    /// Freezes the gameobject, saving its velocity, angular velocity, and contraints. After the freeze time is up, the velocity, angular
    /// velocity, and contraints are returned to the object.
    /// </summary>
    private IEnumerator FreezeObject(float freezeTime)
    {
        unfrozenVelocity = objectPhysics.velocity;
        unfrozenAngularVelocity = objectPhysics.angularVelocity;
        previousContraints = objectPhysics.constraints;
        objectPhysics.constraints = RigidbodyConstraints.FreezeAll;

        //yield return new WaitForSeconds(freezeTime);
        float elapsedTime = 0f;
        while (elapsedTime < freezeTime && complexEntity.IncomingEffect == TimeEffect.None)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        objectPhysics.constraints = previousContraints;
        objectPhysics.velocity = unfrozenVelocity;
        objectPhysics.angularVelocity = unfrozenAngularVelocity;

        complexEntity.TransitionToEffect();
    }

    public override float[] GetData()
    {
        return null;
    }
}
