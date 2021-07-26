using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDownSpeedUpObject : MonoBehaviour
{
    private float slowDuration = 5;
    private bool slowing = false;
    private bool speedingUp = false;
    private float maxSpeed = 1f;
    private float minimumMagnitude = 20f;
    private float speedUpFactor = 2f;
    private WaitForSeconds waitTime = new WaitForSeconds(5f);
    private float radius = 100;
    private float power = 1f;
    private bool slowing2 = false;
    [SerializeField] float slowDownFactor = 0.5f;

    //Both coroutines are called from update to make sure they overwrite the natural velocity
    Rigidbody rb;
    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }
    public void Update()
    {
        StartCoroutine(Slow());

        StartCoroutine(Speed());
    }
    private void FixedUpdate()
    {
        
        if(slowing2)
        {
                rb.AddForce(Physics.gravity * 0.25f, ForceMode.Acceleration);
            
        }
    }
    IEnumerator Slow()
    {
        if (slowing)
        {
 
            slowing2 = true;

                rb.useGravity = false;
                rb.velocity *= slowDownFactor;
                slowing = false;

            if (rb.angularVelocity.sqrMagnitude > maxSpeed)
                rb.angularVelocity = rb.angularVelocity.normalized * maxSpeed;



            //objects are not to be slowed permanently, hence the coroutine timer to have the effect wear off
            yield return waitTime;
            Destroy(gameObject);
            slowing2 = false;
            rb.useGravity = true;
            

        }
    }

     IEnumerator Speed()
    {
        if (speedingUp)
        {
            rb.useGravity = true;
            rb.AddExplosionForce(power, transform.forward, radius, 0F, ForceMode.Impulse);
            // 20 is a close whole number approximation of two times 9.8 (gravity is 9.8 meters per second per second)
            if (rb.velocity.magnitude < minimumMagnitude)
                rb.velocity *= speedUpFactor;
            if(rb.angularVelocity.magnitude < minimumMagnitude)    
                rb.angularVelocity *= speedUpFactor;
            


            yield return waitTime;
            speedingUp = false;
        }
    }

    public void SpeedUp()
    {
        speedingUp = true;
    }

    public void SlowDown()
    {
        slowing = true;
    }

}
