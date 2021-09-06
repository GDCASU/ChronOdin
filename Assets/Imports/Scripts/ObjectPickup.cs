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
    [Tooltip("The camera attached to Player")]
    [SerializeField]
    Transform playerCamera;

    [SerializeField]
    [Tooltip("The maximum distance the Player can pick up an object")]
    private float maxPickupDistance = 10f;

    [SerializeField]
    [Tooltip("The amount of force that can be applied to the held object before Player releases it")]
    private float releaseForce;

    private Transform heldObject = null;  // the transform of the picked up object
    private Rigidbody objectPhysics = null;  // the rigidbody of the picked up object
    private RaycastHit rayHit;  // info about ray collisions
    
    // The previous interpolation and detection mode of the picked up object.
    private RigidbodyInterpolation previousInterpolation = RigidbodyInterpolation.None;
    private CollisionDetectionMode previousDetectionMode = CollisionDetectionMode.Discrete;

    FixedJoint objectJoint;  // the joint instantiated between playerCamera and heldObject

    /// <summary>
    /// Attempts to pick up an object when Player presses down [Tab]. 
    /// Releases object when its joint is broken or the Player ceases pressing down [Tab].
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Pickup();
        }

        if (heldObject != null)
        {
            if ((objectJoint == null) || Input.GetKeyUp(KeyCode.Tab))
            {
                Release();
            }
        }
    }

    /// <summary>
    /// Casts a ray from playerCamera in the forward direction. If the ray hits an object with the "Liftable" tag, the transform
    /// and rigidbody of the object is captured. Then the object's gravity is deactivated, and its interpolation and detection modes
    /// are altered. Then a fixed joint between the playerCamera and object is instantiated with a specified break force.
    /// </summary>
    private void Pickup()
    {
        if(Physics.Raycast(playerCamera.position, playerCamera.TransformDirection(Vector3.forward), out rayHit, maxPickupDistance))
        {
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

                // Add joint between held object and Player camera, and then assign a break force.
                objectJoint = heldObject.gameObject.AddComponent<FixedJoint>();
                objectJoint.connectedBody = playerCamera.GetComponent<Rigidbody>();
                objectJoint.breakForce = releaseForce;
            }
        }
    }

    /// <summary>
    /// Releases the held object from Player's grasp, which restores the object's properties before being
    /// picked up and destroys the joint if it did not break.
    /// </summary>
    private void Release()
    {
        // Restore object properties before pickup.
        objectPhysics.useGravity = true;
        objectPhysics.interpolation = previousInterpolation;
        objectPhysics.collisionDetectionMode = previousDetectionMode;

        // Destroy joint if it did not break.
        if (objectJoint != null)
            Destroy(objectJoint);

        // Assign the held object as nothing.
        heldObject = null;
    }
}
