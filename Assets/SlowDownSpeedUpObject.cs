using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDownSpeedUpObject : MonoBehaviour
{
    private float slowDuration = 5;
    private bool slowing = false;
    private bool speedingUp = false;
    private float maxSpeed = 1f;

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
            //this can get removed in the game, this is only just so that the block falls when shot with a raycast
            rid.useGravity = true;
           
            //changes to velocity are only made if the magnitude is greater than 1, same for the angular velocity
            if (rid.velocity.magnitude > maxSpeed)
                rid.velocity = rid.velocity.normalized * maxSpeed;
            if (rid.angularVelocity.magnitude > maxSpeed)
                rid.angularVelocity = rid.angularVelocity.normalized * maxSpeed;

            //objects are not to be slowed permanently, hence the coroutine timer to have the effect wear off
            yield return new WaitForSeconds(slowDuration);
            slowing = false;
        }
        else
            yield return new WaitForSeconds(0);
    }

     IEnumerator Speed()
    {
        if (speedingUp)
        {
            Rigidbody rid = gameObject.GetComponent<Rigidbody>();
            rid.useGravity = true;
            // 20 is a close whole number approximation of two times 9.8 (gravity is 9.8 meters per second per second)
            if(rid.velocity.magnitude < 20)
                rid.velocity *= 2;
            if(rid.angularVelocity.magnitude < 20)    
                rid.angularVelocity *= 2;
            


            yield return new WaitForSeconds(slowDuration);
            speedingUp = false;
        }
        else
            yield return new WaitForSeconds(0);
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
