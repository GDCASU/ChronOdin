/*
 * Revision Author: Cristion Dominguez
 * Modification: The script has 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : SimpleTimeManipulation
{
    [SerializeField]
    private float rotateSpeed = 2;
    //public GameObject pillar;

    // Update is called once per frame
    void Update()
    {
        rotateSpeed = rotateSpeed * timescale * Time.fixedDeltaTime;
        gameObject.transform.Rotate(0, rotateSpeed, 0, Space.Self);
    }
}