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

    //Both coroutines are called from update to make sure they overwrite the natural velocity
    public void Update()
    {
        StartCoroutine(Slow());

        StartCoroutine(Speed());
    }

    IEnumerator Slow()
    {
        //entering the slow method does nothing if it's not currently slowing
        if (slowing)
        {
            Rigidbody rid = gameObject.GetComponent<Rigidbody>();
            //these two lines can get removed in the game, this is only just so that the block falls when shot with a raycast and/or has a force applied laterally
            rid.useGravity = true;
            rid.AddExplosionForce(power, transform.forward, radius, 0, ForceMode.Impulse);

            //changes to velocity are only made if the magnitude is greater than 1, same for the angular velocity
            if (rid.velocity.sqrMagnitude > maxSpeed)
                rid.velocity *= 0.90f;

            // the same approach to velocity does not have the intended effect, here we normalize the vector and multiply by 1 instead
            if (rid.angularVelocity.sqrMagnitude > maxSpeed)
                rid.angularVelocity = rid.angularVelocity.normalized * maxSpeed;



            //objects are not to be slowed permanently, hence the coroutine timer to have the effect wear off
            yield return waitTime;
            slowing = false;
        }
    }

     IEnumerator Speed()
    {
        if (speedingUp)
        {
            Rigidbody rid = gameObject.GetComponent<Rigidbody>();
            rid.useGravity = true;
            rid.AddExplosionForce(power, transform.forward, radius, 0F, ForceMode.Impulse);
            // 20 is a close whole number approximation of two times 9.8 (gravity is 9.8 meters per second per second)
            if (rid.velocity.magnitude < minimumMagnitude)
                rid.velocity *= speedUpFactor;
            if(rid.angularVelocity.magnitude < minimumMagnitude)    
                rid.angularVelocity *= speedUpFactor;
            


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
