using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The bullet class that damages enemies.
/// </summary>
public class Bullet : MonoBehaviour
{
    public int BulletDamage { get; set; }
    [SerializeField] int bulletTimer = 3;

    /// <summary>
    /// Starts the coroutine to deactivate the bullet after a few seconds.
    /// </summary>
    public void FireThenDeactivate()
    {
        StartCoroutine(DeactivateBullet(bulletTimer));
    }

    /// <summary>
    /// Bullet causes damage to the object it collided with.
    /// </summary>
    /// <param name="collision">The collision the bullet entered.</param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
            collision.gameObject.GetComponent<Enemy>().TakeDamage(BulletDamage);

        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Deactivate the bullet after a few seconds given.
    /// </summary>
    /// <param name="bulletTimer">The number of seconds before deactivation.</param>
    /// <returns>Yields time before deactivation.</returns>
    public IEnumerator DeactivateBullet(int bulletTimer)
    {
        yield return new WaitForSeconds(bulletTimer);
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;     // Resets the bullets direction and speed
        this.gameObject.SetActive(false);
    }
}
