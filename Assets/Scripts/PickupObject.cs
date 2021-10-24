using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script will allow the player to pickup the assigned object.
/// Author: Alben Trang
/// </summary>
public class PickupObject : MonoBehaviour
{
    /// <summary>
    /// The object is picked up by the player and held to its destination.
    /// </summary>
    /// <param name="pickupDest">The place where the object is held by the player.</param>
    public void Pickup(Transform pickupDest)
    {
        this.GetComponent<Rigidbody>().useGravity = false;
        this.GetComponent<Rigidbody>().freezeRotation = true;
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.GetComponent<Collider>().enabled = false;
        this.transform.position = pickupDest.position;
        this.transform.parent = pickupDest.transform;
    }

    /// <summary>
    /// The object is dropped by the player.
    /// </summary>
    /// <param name="pickupDest">The destination that has the object.</param>
    public void Drop(Transform pickupDest)
    {
        pickupDest.GetChild(0).position = pickupDest.position;
        pickupDest.GetChild(0).GetComponent<Rigidbody>().useGravity = true;
        pickupDest.GetChild(0).GetComponent<Rigidbody>().freezeRotation = false;
        this.GetComponent<Collider>().enabled = true;
        pickupDest.DetachChildren();
    }
}
