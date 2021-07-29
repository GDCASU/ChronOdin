using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDownSpeedUpObject : MonoBehaviour
{
    private float slowDuration = 5;
    private bool speedingUp = false;
    private float maxSpeed = 1f;
    private float minimumMagnitude = 20f;
    private float speedUpFactor = 2f;
    private WaitForSeconds waitTime = new WaitForSeconds(5f);
    private bool slowing = false;
    private Vector3 preVelocity;
    [SerializeField] float slowDownFactor = 0.5f;

    //Both coroutines are called from update to make sure they overwrite the natural velocity
    Rigidbody rb;
    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        
        if(slowing)
        {
                rb.AddForce(Physics.gravity * 0.25f, ForceMode.Acceleration);
            
        }

        if (speedingUp)
        {
            rb.AddForce(Physics.gravity / 0.25f, ForceMode.Acceleration);

        }
    }
    IEnumerator Slow()
    {
       
 
            slowing = true;

            rb.useGravity = false;
            preVelocity = rb.velocity;
            rb.velocity *= slowDownFactor;

            rb.angularVelocity *= slowDownFactor;
            



            //objects are not to be slowed permanently, hence the coroutine timer to have the effect wear off
            yield return waitTime;
            //Destroy(gameObject);
            rb.velocity = preVelocity;
            slowing = false;
            rb.useGravity = true;
            

    }

     IEnumerator Speed()
    {
        speedingUp = true;
        //if (rb.velocity.magnitude < minimumMagnitude)
        //    rb.velocity *= speedUpFactor;
        //if(rb.angularVelocity.magnitude < minimumMagnitude)    
        //    rb.angularVelocity *= speedUpFactor;
        rb.useGravity = false;
        preVelocity = rb.velocity;
        rb.velocity /= slowDownFactor;
        rb.angularVelocity /= slowDownFactor;
            


        yield return waitTime;
        //Destroy(gameObject);
        rb.velocity = preVelocity;
        rb.useGravity = true;
        speedingUp = false;
    }

    public void SpeedUp()
    {
        StartCoroutine(Speed());
    }

    public void SlowDown()
    {
        StartCoroutine(Slow());
    }

}
