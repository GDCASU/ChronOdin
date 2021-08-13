using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDownSpeedUpObject : MonoBehaviour
{
    private bool speedingUp = false;
    private WaitForSeconds waitTime = new WaitForSeconds(5f);
    private bool slowing = false;
    private Vector3 preVelocity;
    [SerializeField] float slowDownFactor = 0.5f;
    [SerializeField] float speedUpFactor = 2f;
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
        {
            rb.AddForce(Physics.gravity * (slowDownFactor * slowDownFactor), ForceMode.Acceleration);
            
        }

        if (speedingUp)
        {
            rb.AddForce(Physics.gravity * (speedUpFactor * speedUpFactor), ForceMode.Acceleration);

        }
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
    IEnumerator Slow()
    {
        slowing = true;
        rb.useGravity = false;
        preVelocity = rb.velocity;
        rb.velocity *= slowDownFactor;
        rb.angularVelocity *= slowDownFactor;

        yield return waitTime;
        rb.velocity = preVelocity;
        slowing = false;
        rb.useGravity = true;
        casting = false;
            

    }

     IEnumerator Speed()
    {
        //all the same stuff as the Slow method but the opposite to have a speedup effect
        speedingUp = true;
        rb.useGravity = false;
        preVelocity = rb.velocity;
        rb.velocity *= speedUpFactor;
        rb.angularVelocity *= speedUpFactor;

        yield return waitTime;
        rb.velocity = preVelocity;
        rb.useGravity = true;
        speedingUp = false;
        casting = false;
    }

    public void SpeedUp()
    {
        if (!casting)
        {
            casting = true;
            StartCoroutine(Speed());
        }
    }

    public void SlowDown()
    {
        if(!casting)
        {
            casting = true;
            StartCoroutine(Slow());
        }
    }

}
