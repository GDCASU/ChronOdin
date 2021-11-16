/*
 * Revision Author: Cristion Dominguez
 * Modification: 
 *  Subscribed the SlowDown method to the slowEveryObject event upon Scene start.
 *  The slowDownFactor is set in the SlowInvocation script.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDownSpeedUpObject : ComplexSlow
{
    private bool speedingUp = false;
    private WaitForSeconds waitTime = new WaitForSeconds(5f);
    private bool slowing = false;
    private Vector3 preVelocity;
    private float slowDownFactor = 0.5f;
    private float speedUpFactor = 2f;
    private bool casting = false;

    Rigidbody rb;
    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // this adds artificial gravity since we are disabling the in game gravity as part of acheiving our effect
        if(slowing)
            rb.AddForce(Physics.gravity * (slowDownFactor * slowDownFactor), ForceMode.Acceleration);
        if(speedingUp)
            rb.AddForce(Physics.gravity * (speedUpFactor * speedUpFactor), ForceMode.Acceleration);

        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    SlowDown();
        //}
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    SpeedUp();
        //}
    }
    public bool GetSpeedingStatus()
    {
        return speedingUp;
    }
    public bool GetSlowingStatus()
    {
        return slowing;
    }
    // Save velocity and turn off gravity for the object reduce velocity and angular velocity
    IEnumerator SlowObject(float slowTime)
    {
        slowing = true;
        rb.useGravity = false;
        preVelocity = rb.velocity;
        rb.velocity *= slowDownFactor;
        rb.angularVelocity *= slowDownFactor;

        float elapsedTime = 0f;
        while (elapsedTime < slowTime && complexEntity.NewEffect == TimeEffect.None)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        //yield return new WaitForSeconds(slowTime);

        rb.velocity = preVelocity;
        slowing = false;
        rb.useGravity = true;
        casting = false;

        complexEntity.ResetCurrentTimeEffect();
    }

     IEnumerator SpeedObject()
    {
        //all the same stuff as the Slow method but the opposite to have a speedup effect
        speedingUp = true;
        rb.useGravity = false;
        preVelocity = rb.velocity;
        rb.velocity *= speedUpFactor;
        rb.angularVelocity *= speedUpFactor;

        yield return waitTime;
        rb.velocity = preVelocity;
        speedingUp = false;
        rb.useGravity = true;
        casting = false;
    }

    public override void Slow(float slowTime, float newSlowDownFactor)
    {
        slowDownFactor = newSlowDownFactor;

        if(!casting)
        {
            casting = true;
            StartCoroutine(SlowObject(slowTime));
        }
    }

    public void SpeedUp()
    {
        if (!casting)
        {
            casting = true;
            StartCoroutine(SpeedObject());
        }
    }

    public override float[] GetData()
    {
        return null;
    }
}
