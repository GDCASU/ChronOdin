using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    Rigidbody rb;
    public float multiplier;
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Vector3 dirToCenter = transform.parent.position - collision.transform.position;
            Vector3 projection = Vector3.Project(dirToCenter, -transform.forward);
            rb.velocity += transform.right * multiplier * projection.magnitude * GetComponentInParent<RotateObject>().TimeScale;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player") rb = collision.gameObject.GetComponent<Rigidbody>();   
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player") rb = null;
    }
}
