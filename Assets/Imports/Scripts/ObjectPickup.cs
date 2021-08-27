using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPickup : MonoBehaviour
{
    [SerializeField]
    Transform playerCamera;

    [SerializeField]
    Transform pickupDestination;

    [SerializeField]
    [Tooltip("")]
    private float maxPickupDistance = 10f;

    [SerializeField]
    [Tooltip("")]
    private float returnSpeed = 0.1f;

    private Transform heldObject = null;
    private Rigidbody objectPhysics = null;
    private Collider objectCollider = null;
    private RaycastHit rayHit;

    private RigidbodyInterpolation previousInterpolation = RigidbodyInterpolation.None;
    private CollisionDetectionMode previousDetectionMode = CollisionDetectionMode.Discrete;
    private Vector3 previousPosition = Vector3.zero;
    float timeSinceLastSave = 0f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Pickup();
        }
        else if(Input.GetKeyUp(KeyCode.Tab))
        {
            Release();
        }

        /*
        if (heldObject != null)
        {
            if (Physics.Raycast(previousPosition, heldObject.position - previousPosition, out rayHit, Vector3.Distance(previousPosition, heldObject.position)))
            {
                if (!rayHit.collider.Equals(objectCollider))
                {
                    heldObject.LookAt(rayHit.normal, Vector3.up);
                    heldObject.position = rayHit.point + rayHit.normal * heldObject.lossyScale.y / 2;

                    previousPosition = heldObject.position;
                    timeSinceLastSave = 0;
                }
            }

            if (timeSinceLastSave >= 0.5f)
            {
                previousPosition = heldObject.position;
                timeSinceLastSave = 0;
            }

            timeSinceLastSave += Time.deltaTime;
        }
        */
    }

    private void FixedUpdate()
    {
        if(heldObject != null)
        {
            //heldObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            //heldObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            if (heldObject.position != pickupDestination.position)
            {
                //heldObject.position = Vector3.MoveTowards(heldObject.position, pickupDestination.position, returnSpeed);

                Vector3 dir = pickupDestination.position - heldObject.position;
                //objectPhysics.velocity = dir.normalized * returnSpeed;

                objectPhysics.MovePosition(pickupDestination.position);
            }

            if (Vector3.Distance(heldObject.position, playerCamera.position) > maxPickupDistance)
            {
                Release();
            }
        }
    }

    private void Pickup()
    {
        if(Physics.Raycast(playerCamera.position, playerCamera.TransformDirection(Vector3.forward), out rayHit, maxPickupDistance, 1<<6))
        {
            heldObject = rayHit.transform;
            objectPhysics = heldObject.GetComponent<Rigidbody>();
            objectCollider = heldObject.GetComponent<Collider>();

            objectPhysics.useGravity = false;
            previousInterpolation = objectPhysics.interpolation;
            previousDetectionMode = objectPhysics.collisionDetectionMode;
            objectPhysics.interpolation = RigidbodyInterpolation.Interpolate;
            objectPhysics.collisionDetectionMode = CollisionDetectionMode.Continuous;

            //heldObject.parent = playerCamera;
            pickupDestination.position = heldObject.position;

            previousPosition = heldObject.position;
            timeSinceLastSave = 0;
        }
    }

    private void Release()
    {
        if(heldObject != null)
        {
            objectPhysics.useGravity = true;
            objectPhysics.interpolation = previousInterpolation;
            objectPhysics.collisionDetectionMode = previousDetectionMode;

            heldObject.parent = null;
            heldObject = null;
        }
    }
}
