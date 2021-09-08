/*
 * Allows the Player to pick up liftable objects upon pressing [Tab].
 * Objects are released when Player ceases pressing [Tab].
 * 
 * Author: Cristion Dominguez
 * Date: 6 Sept. 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPickup : MonoBehaviour
{
    [Header("Transforms")]
    [Tooltip("The camera attached to Player")]
    [SerializeField]
    Transform playerCamera;

    [Tooltip("An empty Transform that the held object shall follow")]
    [SerializeField]
    Transform target;

    [Header("Properties")]
    [SerializeField]
    [Tooltip("The maximum distance the Player can pick up an object")]
    private float maxPickupDistance = 10f;

    [SerializeField]
    [Tooltip("The speed the held object travels to the target")]
    private float speed = 1750f;

    private Transform heldObject = null;  // the transform of the picked up object
    private Rigidbody objectPhysics = null;  // the rigidbody of the picked up object
    private RaycastHit rayHit;  // info about ray collisions
    
    // The previous interpolation and detection mode of the picked up object.
    private RigidbodyInterpolation previousInterpolation = RigidbodyInterpolation.None;
    private CollisionDetectionMode previousDetectionMode = CollisionDetectionMode.Discrete;

    // Values for calculating held object velocity.
    Vector3 direction;
    float distance;

    /// <summary>
    /// Attempts to pick up an object when Player presses down [Tab]. 
    /// Releases object when the Player ceases pressing down [Tab].
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Pickup();
        }

        if (heldObject != null)
        {
            if (Input.GetKeyUp(KeyCode.Tab))
            {
                Release();
            }
        }
    }

    /// <summary>
    /// Grants the held object velocity based on the direction to target, distance from target, and speed.
    /// </summary>
    private void FixedUpdate()
    {
        if (heldObject != null)
        {
            direction = (target.position - objectPhysics.position).normalized;
            distance = Vector3.Distance(target.position, objectPhysics.position);
            objectPhysics.velocity = direction * distance * speed * Time.fixedDeltaTime;            
        }
    }

    /// <summary>
    /// Casts a ray from playerCamera in the forward direction. If the ray hits an object with the "Liftable" tag, the transform
    /// and rigidbody of the object is captured. Then the object's gravity is deactivated, and its interpolation and detection modes
    /// are altered. Then the target position is centered at the held object's current position.
    /// </summary>
    private void Pickup()
    {
        if(Physics.Raycast(playerCamera.position, playerCamera.TransformDirection(Vector3.forward), out rayHit, maxPickupDistance))
        {
            print(rayHit.transform.name);
            if (rayHit.transform.tag.Equals("Liftable"))
            {
                // Capture the object's transform and rigidbody.
                heldObject = rayHit.transform;
                objectPhysics = heldObject.GetComponent<Rigidbody>();

                // Alter the object's physics properties.
                objectPhysics.useGravity = false;
                previousInterpolation = objectPhysics.interpolation;
                previousDetectionMode = objectPhysics.collisionDetectionMode;
                objectPhysics.interpolation = RigidbodyInterpolation.Interpolate;
                objectPhysics.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

                // Center target at held object.
                target.position = heldObject.position;
            }
        }
    }

    /// <summary>
    /// Releases the held object from Player's grasp, which restores the object's properties before being
    /// picked up.
    /// </summary>
    private void Release()
    {
        // Restore object properties before pickup.
        objectPhysics.useGravity = true;
        objectPhysics.interpolation = previousInterpolation;
        objectPhysics.collisionDetectionMode = previousDetectionMode;

        // Assign the held object as nothing.
        heldObject = null;
    }
}
