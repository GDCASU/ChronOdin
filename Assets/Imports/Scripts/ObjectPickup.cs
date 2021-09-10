/*
 * Allows the Player to pick up liftable objects upon pressing the pickup key.
 * Objects are released when Player ceases pressing the pickup key.
 * 
 * Author: Cristion Dominguez
 * Date: 6 Sept. 2021
 */

using UnityEngine;

public class ObjectPickup : MonoBehaviour
{
    [Header("Button")]
    [Tooltip("Button to invoke object pickup")]
    [SerializeField]
    private KeyCode pickupKey = KeyCode.Tab;

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
    private float maxPickupDistance = 2f;

    [SerializeField]
    [Tooltip("The speed the held object travels to the target")]
    private float speed = 1500f;

    private Transform heldObject = null;  // the transform of the picked up object
    private Rigidbody objectPhysics = null;  // the rigidbody of the picked up object
    private RaycastHit rayHit;  // info about ray collisions
    
    // The previous interpolation and detection mode of the picked up object.
    private RigidbodyInterpolation previousInterpolation = RigidbodyInterpolation.None;
    private CollisionDetectionMode previousDetectionMode = CollisionDetectionMode.Discrete;

    // Values for casting a ray to detect collisions.
    Vector3 startRayPosition;  // starting position of ray
    Vector3 rayDirection;  // direction of ray
    float rayPositionOffset = 0.000006f;  // distance the new starting position is away from the hit position if the ray detected the Player
    int maxRayCasts = 2;  // max amount of times to cast ray

    // Values for calculating held object velocity.
    Vector3 directionToTarget;  // direction from held object to target
    float distanceToTarget;  // distance between held object and target

    /// <summary>
    /// Attempts to pick up an object when Player presses down the pickup key. 
    /// Releases object when the Player ceases pressing down the pickup key.
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            Pickup();
        }

        if (heldObject != null)
        {
            if (Input.GetKeyUp(pickupKey))
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
            directionToTarget = (target.position - objectPhysics.position).normalized;
            distanceToTarget = Vector3.Distance(target.position, objectPhysics.position);
            objectPhysics.velocity = directionToTarget * distanceToTarget * speed * Time.fixedDeltaTime;
            Debug.Log(objectPhysics.velocity.magnitude);
        }
    }

    /// <summary>
    /// Casts a ray from playerCamera in the forward direction.
    /// If the ray hits the Player, another ray is casted a distance away (rayPositionOffset) from the hit position in the previous ray's direction.
    ///   This can be done for a finite amount of times (maxRayCasts).
    /// If the ray hits an object with the "Liftable" tag, the transform and rigidbody of the object is captured. Then the object's gravity is deactivated,
    ///   and its interpolation and detection modes are altered. Then the target position is centered at the held object's current position.
    /// If the ray hits no collider, nothing occurs.
    /// </summary>
    private void Pickup()
    {
        // Set the ray's starting position and direction.
        startRayPosition = playerCamera.position;
        rayDirection = playerCamera.TransformDirection(Vector3.forward);

        // Cast the ray until the ray does not hit the Player or maxRayCasts has been reached.
        for(int i = 0; i < maxRayCasts; i++)
        {
            if (Physics.Raycast(startRayPosition, rayDirection, out rayHit, maxPickupDistance))
            {
                // If the ray hits the Player, re-assign the starting position to be a bit away from the hit position
                // in the previous ray's direction and continue to the next loop iteration.
                if (rayHit.transform.tag.Equals("Player"))
                {
                    startRayPosition = rayHit.point + (rayDirection.normalized * rayPositionOffset);
                    continue;
                }
                // If the ray hits a Liftable, prepare the object for holding and stop casting rays.
                else if (rayHit.transform.tag.Equals("Liftable"))
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
                    return;
                }
                // If the ray hits nothing, stop casting rays.
                // FOR TESTING PURPOSES, comment this "else" block out.
                else
                {
                    return;
                }
            }
        }

        // FOR TESTING PURPOSES, remove the comments for the block below.
        /*
         if (rayHit.transform != null)
            Debug.Log(rayHit.transform.name);
         else
            Debug.Log("N/A");
        */
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
