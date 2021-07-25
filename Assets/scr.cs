using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr : MonoBehaviour
{
    private float maxSpeed = 1f;
    private float minSpeed = -1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (gameObject.GetComponent<Rigidbody>().velocity.y <= -1)
        //    gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, -1, 0);
        Rigidbody2D rid = gameObject.GetComponent<Rigidbody2D>();
        if (rid.velocity.magnitude > maxSpeed)
            rid.velocity = rid.velocity.normalized * maxSpeed;
        if (rid.velocity.magnitude < minSpeed)
           rid.velocity = rid.velocity.normalized * minSpeed;
    }
}
