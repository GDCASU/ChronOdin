using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController
{
    [System.Serializable]
    public class DownLungeVariables
    {
        public float lungeForce = 50;
        public bool lungedUsed;
        public float lungeDuration = .5f;
        public float minDistanceToGround = 5;
        public float minTimeSinceGround = .3f;
    }
    private IEnumerator DownLunge()
    {
        if (rb.velocity.y > 0) rb.velocity -= Vector3.up * rb.velocity.y;
        rb.velocity += -Vector3.up * downLungeVariables.lungeForce;
        downLungeVariables.lungedUsed = true;
        yield return new WaitForSeconds(downLungeVariables.lungeDuration);
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        SetInitialGravity();
    }
}
