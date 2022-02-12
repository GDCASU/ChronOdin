using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRigidbodyFreeze : ComplexFreeze
{
    private Vector3 previousVelocity;
    private Rigidbody rb;

    private void Start() => rb = GetComponent<Rigidbody>();
    public override void Freeze(float freezeTime) => StartCoroutine(FreezeRigidBody(freezeTime));

    IEnumerator FreezeRigidBody(float freezeTime)
    {
        previousVelocity = rb.velocity;
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
        float timer = freezeTime;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        rb.useGravity = true;
        rb.velocity = previousVelocity;
        effectHub.CurrentEffect = TimeEffect.None;
    }
}