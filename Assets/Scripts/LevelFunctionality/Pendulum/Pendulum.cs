/*
 * A pendulum that sets its rigidbody rotation every fixed update.
 * Whenever an entity collides with the ball, a force is applied to that entity.
 * 
 * Author: Cristion Dominguez
 * Date: 21 Dec. 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : SimpleTimeManipulation
{
    [Header("Swing")]
    [SerializeField, Range(0f, 179f), Tooltip("The degree the pendulum ball is dropped when time = 0 seconds.")]
    private float initialAngle = 90f;

    [SerializeField, Tooltip("How quickly the pendulum ball swings side to side.")]
    private float angularFrequency = 0.5f;

    //[SerializeField, Tooltip("The time (in seconds) after the pendulum ball was dropped; resets to 0 when the ball reaches its initial position.")]
    private float time = 0f;

    [Header("Collision")]
    [SerializeField, Range(0f, 360f), Tooltip("The maximum angle a collided entity can be from the ball's velocity vector before the min hit force is always applied.")]
    private float maxHitAngle = 90f;

    [SerializeField, Tooltip("The maximum amount of force to apply when the ball hits the player.")]
    private float maxPlayerHitForce = 4000f;

    [SerializeField, Tooltip("The minimum amount of force to apply when the ball hits the player.")]
    private float minPlayerHitForce = 1f;

    [SerializeField, Tooltip("The maximum amount of force to apply when the ball hits an inanimate object.")]
    private float maxObjectHitForce = 1000f;

    [SerializeField, Tooltip("The minimum amount of force to apply when the ball hits an inanimate object.")]
    private float minObjectHitForce = 1f;

    private Rigidbody pendulumBody;
    private Collider ballCollider;
    private List<ContactPoint> collisionContacts;  // points of contact upon collision with pendulum
    
    private float currentAngle;
    private float maxBallVelocity;
    private bool wasTimeResetSinceMaxHeight = false;  // Was the time reset since the ball reached max height?

    private Vector3 initialRotation;

    /// <summary>
    /// Acquires the rigidbody of the pendulum, sets its center of mass, acquires the ball's collider, calculates the max velocity of the ball, and initializes collision contacts.
    /// </summary>
    private void Awake()
    {
        pendulumBody = GetComponent<Rigidbody>();
        pendulumBody.centerOfMass = new Vector3(0, 0, 0);

        ballCollider = transform.GetChild(1).GetComponent<Collider>();

        maxBallVelocity = (initialAngle * Mathf.Deg2Rad) * (-Physics.gravity.y / angularFrequency);

        collisionContacts = new List<ContactPoint>();
    }
    /// <summary>
    /// Updates the local timeScale variable before the first update is called
    /// </summary>
    private void Start() 
    {
        UpdateTimeScale(MasterTime.singleton.timeScale);
        initialRotation = transform.localRotation.eulerAngles;
    } 
    /// <summary>
    /// Calculates the angle the ball should be at with respect to the y-axis.
    /// </summary>
    private void FixedUpdate()
    {
        currentAngle = initialAngle * Mathf.Cos(angularFrequency * time);
        pendulumBody.MoveRotation(Quaternion.Euler(initialRotation + Vector3.forward * currentAngle));

        // Ensure time does not grow too large.
        if (time >= 0f)
        {
            if (!wasTimeResetSinceMaxHeight && (initialAngle - currentAngle) <= 0.002f)
            {
                time = 0f;
                wasTimeResetSinceMaxHeight = true;
            }
            else if ((initialAngle + currentAngle) <= 0.002f)
            {
                wasTimeResetSinceMaxHeight = false;
            }
        }

        time += (Time.fixedDeltaTime * _timeScale);
    }

    /// <summary>
    /// Applies a force to whatever entity collides with the ball, as long as the entity has a rigidbody.
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        // Do not apply a force if the pendulum is frozen.
        if (_timeScale == 0f)
            return;

        // Only apply a force to an object with a rigidbody.
        Rigidbody collidedBody = collision.transform.GetComponent<Rigidbody>();
        if (collidedBody == null)
        {
            return;
        }

        // Only apply a force if the entity collides with the ball.
        if (collision.GetContacts(collisionContacts) > 0)
        {
            foreach (ContactPoint contact in collisionContacts)
            {
                if (contact.thisCollider.Equals(ballCollider))
                {
                    goto CalculateHitForce;
                }
            }
        }
        return;

        CalculateHitForce:
        // Calculate the direction from the ball to the entity and collect the entity's rigidbody.
        Vector3 collideDir = (collision.transform.position - ballCollider.transform.position).normalized;
        
        // Calculate the velocity of the ball.
        Vector3 angularVelocity = transform.forward * -(initialAngle * Mathf.Deg2Rad) * angularFrequency * Mathf.Sin(angularFrequency * time);
        Vector3 radius = (ballCollider.transform.position - transform.position).normalized * -Physics.gravity.y * (1f / Mathf.Pow(angularFrequency, 2));
        Vector3 velocity = Vector3.Cross(angularVelocity, radius) * _timeScale;

        // Calculate the launch direction of the entity.
        Vector3 launchDirection = (collideDir + velocity).normalized;

        // Calculate the angle between the collide direction and the velocity.
        float collisionAngle = Vector3.Angle(collideDir, velocity);

        // Determine which min and max hit force values to utilize.
        // The force magnitude is determined by the velocity of the ball, the angle the entity collided with the ball, and the hit forces.
        float forceMagnitude = 0;
        if (collision.transform.CompareTag("Player"))
        {
            forceMagnitude = (velocity.magnitude * (1f / (maxBallVelocity == 0 ? .001f : maxBallVelocity) ) ) * Mathf.Lerp(maxPlayerHitForce, minPlayerHitForce, collisionAngle * (1f / 90f));
            collidedBody.AddForce(launchDirection * forceMagnitude);
        }
        else if (collidedBody != null)
        {
            forceMagnitude = (velocity.magnitude * (1f / (maxBallVelocity == 0 ? .001f : maxBallVelocity) ) ) * Mathf.Lerp(maxObjectHitForce, minObjectHitForce, collisionAngle * (1f / 90f));
            collidedBody.AddForce(launchDirection * forceMagnitude);
        }
    }
}
