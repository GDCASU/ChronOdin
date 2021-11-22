/*
 * Allows the Player to pick up liftable objects upon pressing [Tab].
 * Objects are released when Player ceases pressing [Tab].
 * 
 * Author: Cristion Dominguez
 * Date: 6 Sept. 2021
 */

using UnityEngine;
public class ObjectPickup : MonoBehaviour
{
    [Header("Transforms")]

    [Tooltip("An empty Transform that the held object shall follow")]
    [SerializeField]
    Transform target;

    [Header("Properties")]
    [SerializeField]
    [Tooltip("The distance the object will be from teh player after being pick up")]
    private float distanceFromPlayer = 3f;

    [SerializeField]
    [Tooltip("The speed the held object travels to the target")]
    private float speed = 1500f;

    [SerializeField]
    [Tooltip("Layer the object will be set to to avoid collisions with the palyer")]
    private int newLayer = 7;                   //7 is the ignore player layer

    private int originalLayer;

    private Transform heldObject = null;  // the transform of the picked up object
    private Rigidbody objectRb = null;  // the rigidbody of the picked up object
    
    // The previous interpolation and detection mode of the picked up object.
    private RigidbodyInterpolation previousInterpolation = RigidbodyInterpolation.None;
    private CollisionDetectionMode previousDetectionMode = CollisionDetectionMode.Discrete;

    // Values for calculating held object velocity.
    Vector3 directionToTarget;  // direction from held object to target
    float distanceToTarget;  // distance between held object and target

    /// <summary>
    /// Grants the held object velocity based on the direction to target, distance from target, and speed.
    /// </summary>
    private void FixedUpdate()
    {
        if (heldObject != null)
        {
            directionToTarget = (target.position - objectRb.position).normalized;
            distanceToTarget = Vector3.Distance(target.position, objectRb.position);
            objectRb.velocity = directionToTarget * distanceToTarget * speed * Time.fixedDeltaTime;
            Vector3 dirToPlayer = (transform.position - heldObject.position);
            Physics.Raycast(heldObject.position, dirToPlayer.normalized, out RaycastHit rayHit, dirToPlayer.magnitude);
            Debug.DrawLine(heldObject.position, heldObject.position+dirToPlayer,Color.red);
            if (rayHit.collider != null && !rayHit.transform.CompareTag("Player")) ReleaseObject();
            //print(Vector3.Angle(-transform.up, directionToTarget));
            //Debug.Log(objectPhysics.velocity.magnitude);
        }
    }
    /// <summary>
    ///The transform and rigidbody of the object is captured. Then the object's gravity is deactivated,
    ///   and its interpolation and detection modes are altered. Then the target position is centered at the held object's current position.
    /// If the ray hits no collider, nothing occurs.
    /// </summary>
    public void PickupObject()
    {
        // If the ray hits a Liftable, prepare the object for holding and stop casting rays.
        if (PlayerInteractions.singleton.rayHit.transform.tag.Equals("Liftable"))
        {
            // Capture the object's transform and rigidbod
            heldObject = PlayerInteractions.singleton.rayHit.transform;
            //print(Vector3.Angle(-transform.up, (heldObject.position-Camera.main.transform.position).normalized));
            objectRb = heldObject.GetComponent<Rigidbody>();
            originalLayer = heldObject.gameObject.layer;
            heldObject.gameObject.layer = newLayer;

            // Alter the object's physics properties.
            objectRb.useGravity = false;
            previousInterpolation = objectRb.interpolation;
            previousDetectionMode = objectRb.collisionDetectionMode;
            objectRb.interpolation = RigidbodyInterpolation.Interpolate;
            objectRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            objectRb.freezeRotation = true;

            // Center target at held object.
            target.position = PlayerInteractions.singleton.playerCamera.transform.position + PlayerInteractions.singleton.playerCamera.transform.forward * distanceFromPlayer;
            return;
        }
        // If the ray hits nothing, stop casting rays.
        // FOR TESTING PURPOSES, comment this "else" block out.
        else if (PlayerInteractions.singleton.rayHit.transform.tag.Equals("Interactable")) PlayerInteractions.singleton.rayHit.transform.GetComponent<InteractiveObject>().Interact();
    }

    /// <summary>
    /// Releases the held object from Player's grasp, which restores the object's properties before being
    /// picked up.
    /// </summary>
    public void ReleaseObject()
    {
        // Restore object properties before pickup.
        objectRb.useGravity = true;
        objectRb.interpolation = previousInterpolation;
        objectRb.collisionDetectionMode = previousDetectionMode;
        objectRb.freezeRotation = false;
        objectRb.gameObject.layer = originalLayer;
        originalLayer = 0;
        // Assign the held object as nothing.
        heldObject = null;
    }
}
