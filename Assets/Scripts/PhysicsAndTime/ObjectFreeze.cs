/*
 * Freezes an attached gameobject with a Rigidbody for a specified amount of time.
 * 
 * Author: Cristion Dominguez
 * Date: 15 September 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFreeze : ComplexFreeze
{
    private Rigidbody objectPhysics;  // for collecting and saving velocity, angular velocity, and rigidbody constraints

    // Save before freezing gameobject.
    private Vector3 unfrozenVelocity, unfrozenAngularVelocity;
    private RigidbodyConstraints previousConstraints;

    /// <summary>
    /// Collects the attached object's rigidbody.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        objectPhysics = transform.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Commences the coroutine for freezing the gameobject.
    /// </summary>
    /// <param name="freezeTime"> time to freeze object </param>
    public override void Freeze(float freezeTime)
    {
        StartCoroutine(FreezeObject(freezeTime));
    }

    /// <summary>
    /// Freezes the gameobject, saving its velocity, angular velocity, and constraints. After the freeze time is up, the velocity, angular
    /// velocity, and constraints are returned to the object.
    /// If the effect hub communicates that a new effect was introduced, then the object is unfrozen and the next effect is transitioned to.
    /// Environment Stacking: environment freeze increases the time the object is frozen for for and environment slow increases it by less.
    /// </summary>
    private IEnumerator FreezeObject(float freezeTime)
    {
        unfrozenVelocity = objectPhysics.velocity;
        unfrozenAngularVelocity = objectPhysics.angularVelocity;
        previousConstraints = objectPhysics.constraints;
        objectPhysics.constraints = RigidbodyConstraints.FreezeAll;

        float elapsedTime = 0f;
        while (elapsedTime < freezeTime && effectHub.IntroducingNewEffect == false)
        {
            elapsedTime += Time.deltaTime * MasterTime.singleton.timeScale;
            yield return null;
        }

        objectPhysics.constraints = previousConstraints;
        objectPhysics.velocity = unfrozenVelocity;
        objectPhysics.angularVelocity = unfrozenAngularVelocity;

        effectHub.TransitionToNextEffect();
    }
}
