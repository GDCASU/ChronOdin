using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Refills the player's gun's ammo.
/// This class is temporary and should be removed from the object when
/// the player can get the ammo pickup objects by using raycasts.
/// </summary>
public class AmmoPickup : MonoBehaviour
{
    /// <summary>
    /// Simple method for refilling the player's gun's ammo when the player collides with it by trigger.
    /// </summary>
    /// <param name="other">The other game object colliding with the ammo pickup.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            other.GetComponent<PlayerGuns>().RefillGuns();

        Destroy(gameObject);
    }
}
