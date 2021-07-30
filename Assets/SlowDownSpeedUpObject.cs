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
            rb.AddForce(Physics.gravity / (speedUpFactor * speedUpFactor), ForceMode.Acceleration);

        }
    }
    IEnumerator Slow()
    {
       
        //this boolean allows us to keep applying the artificial gravity while applying the effect
        slowing = true;
        //this has to be here or you get a null reference from the alt mode launchers
        rb = gameObject.GetComponent<Rigidbody>();
        //here we actually shut off the gravity for the object for the length of the effect
        rb.useGravity = false;
        //we save the original velicity to reinstate after the effect wears off, if that is desired. if not this line can be commented out
        preVelocity = rb.velocity;
        // we actually slow the cube
        rb.velocity *= slowDownFactor;
        // we slow the rotational velocity of the cube
        rb.angularVelocity *= slowDownFactor;
            



            //objects are not to be slowed permanently, hence the coroutine timer to have the effect wear off
            yield return waitTime;
            //we revert our changes once the effect wears off
            rb.velocity = preVelocity;
            slowing = false;
            rb.useGravity = true;
            

    }

     IEnumerator Speed()
    {
        //all tdhe same stuff as the Slow method but the opposite to have a speedup effect
        rb = gameObject.GetComponent<Rigidbody>();
        speedingUp = true;
        rb.useGravity = false;
        preVelocity = rb.velocity;
        rb.velocity /= slowDownFactor;
        rb.angularVelocity /= slowDownFactor;
            


        yield return waitTime;
        rb.velocity = preVelocity;
        rb.useGravity = true;
        speedingUp = false;
    }

    public void SpeedUp()
    {
        //could not figure out how to call the coroutine directly from the PlayerController or Launcher class
        StartCoroutine(Speed());
    }

    public void SlowDown()
    {
        StartCoroutine(Slow());
    }

}
