using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    private Rigidbody rb;
    public float multiplier;
    [Range(0,1)]
    public float rightPreference;
    [Range(0, 1)]
    public float forwardPreference;
    private float lastTimeScale;
    private float parentTimeScale;

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Vector3 dirToCenter = transform.parent.position - collision.transform.position;
            Vector3 projection = Vector3.Project(dirToCenter, -transform.forward);
            parentTimeScale = GetComponentInParent<RotateObject>().TimeScale;
            if (lastTimeScale > parentTimeScale) rb.velocity -= (transform.right * rightPreference - transform.forward * forwardPreference) * multiplier * projection.magnitude * parentTimeScale * 10;
            else rb.velocity += (transform.right * rightPreference - transform.forward * forwardPreference) * multiplier * projection.magnitude * parentTimeScale;
            lastTimeScale = parentTimeScale;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Vector3 dirToCenter = transform.parent.position - collision.transform.position;
            Vector3 projection = Vector3.Project(dirToCenter, -transform.forward);
            parentTimeScale = GetComponentInParent<RotateObject>().TimeScale;
            rb = collision.gameObject.GetComponent<Rigidbody>();
            rb.velocity = (transform.right * rightPreference - transform.forward * forwardPreference) * multiplier * projection.magnitude * parentTimeScale;
        }    
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player") rb = null;
    }
}
