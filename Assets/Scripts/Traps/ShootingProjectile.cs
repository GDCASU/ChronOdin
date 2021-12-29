/*
 * A projectile of the ShootingTrap that travels through air in 1 direction.
 * 
 * Author: Cristion Dominguez
 * Date: 16 Dec. 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingProjectile : SimpleTimeManipulation
{
    [SerializeField, Tooltip("The speed the projectile travels with.")]
    private float speed = 5f;

    [SerializeField, Tooltip("The time the bullet lasts whilst not under any time effect.")]
    private float lifespan = 3f;

    private Rigidbody body;  // Rigidbody of projectile
    private Coroutine launchCoroutine;  // coroutine facilitating the projectile's travel in air

    /// <summary>
    /// Collects the projectile's Rigidbody and rotates the projectile.
    /// </summary>
    private void Awake()
    {
        UpdateTimescale(MasterTime.singleton.timeScale);
        body = GetComponent<Rigidbody>();
        transform.Rotate(90f, 0f, 0f);
    }

    /// <summary>
    /// Alters the projectile's velocity based on the new time scale.
    /// </summary>
    /// <param name="newTimeScale"></param>
    public override void UpdateTimescale(float newTimeScale)
    {
        base.UpdateTimescale(newTimeScale);
        body.velocity = transform.TransformDirection(Vector3.up) * speed * timeScale;
    }

    /// <summary>
    /// Grants the projectile a velocity based on speed and time scale.
    /// </summary>
    public void launch()
    {
        launchCoroutine = StartCoroutine(launchProjectile());
    }
    public IEnumerator launchProjectile()
    {
        body.velocity = transform.TransformDirection(Vector3.up) * speed * timeScale;

        float elapsedTime = 0f;
        while (elapsedTime < lifespan)
        {
            elapsedTime += (Time.deltaTime * timeScale);

            // Prevent the reverse effect from extending the projectile's lifespan outside a time effect.
            // For example, if the projectile has a lifespan of 3 seconds and the reverse effect lasts for 5 seconds, the bullet shall exist for a maximum of 7 seconds.
            if (elapsedTime <= 0)
            {
                elapsedTime = 0f;
            }

            yield return null;
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Kills the Player if the projectile collides with a Player.
    /// Destroys the projectile if it collides with any collider.
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag.Equals("Player"))
        {
            Debug.Log("You died.");
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Stops the launch coroutine when the projectile is disabled.
    /// </summary>
    protected override void OnDisable()
    {
        base.OnDisable();
        StopCoroutine(launchCoroutine);
    }
}
