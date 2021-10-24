using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script lets the player interact with objects in the scene such as doors or pickup objects.
/// Author: Alben Trang
/// </summary>
public class PlayerInteractions : MonoBehaviour
{
    [Tooltip("The player's first-person camera is used as a reference point for interactions")]
    public Camera playerCamera;
    [Tooltip("Set an empty game object in front and as a child of the player camera to hold objects")]
    public Transform pickupDest;
    [Tooltip("Set how far the player can interact with objects with the press of Fire1 (left mouse button)")]
    public float range = 10.0f;

    private bool isHolding;

    /// <summary>
    /// Start on frame on that the player isn't holding anything
    /// </summary>
    private void Start()
    {
        isHolding = false;
    }

    /// <summary>
    /// Check every frame to see if the player hits the Fire1 button (left mouse button) to interact with objects
    /// </summary>
    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (!isHolding && Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, range))
            {
                // Interact with a door
                if (hit.transform.gameObject.GetComponent<Door>())
                {
                    InteractDoor(hit);
                }
                // Interact with pickup object
                else if (hit.transform.gameObject.GetComponent<PickupObject>())
                {
                    PickUpObject(hit);
                } 
            }
            else if (isHolding)
            {
                DropObject();
            }
        }
    }

    /// <summary>
    /// Interacts with the door to open or close it
    /// </summary>
    /// <param name="hit">The object the camera is pointing at</param>
    private void InteractDoor(RaycastHit hit)
    {
        hit.transform.gameObject.GetComponent<Door>().InteractWithDoor();
    }

    /// <summary>
    /// Picks up a pickup object that has the PickupObject script attatched to it
    /// </summary>
    /// <param name="hit">The object the camera is pointing at</param>
    private void PickUpObject(RaycastHit hit)
    {
        hit.transform.gameObject.GetComponent<PickupObject>().Pickup(pickupDest);
        isHolding = true;
    }

    /// <summary>
    /// Drop the object the player is holding
    /// </summary>
    private void DropObject()
    {
        Collider[] colliders = Physics.OverlapBox(pickupDest.GetChild(0).transform.position, pickupDest.GetChild(0).transform.localScale / 2);
        if (colliders.Length == 0) // Makes sure that the pickup object isn't colliding with anything else before being dropped
        { 
            pickupDest.GetChild(0).transform.gameObject.GetComponent<PickupObject>().Drop(pickupDest);
            isHolding = false;
        }
    }
}
