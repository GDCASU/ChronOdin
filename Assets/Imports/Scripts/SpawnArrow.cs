using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns an arrow above the button pressed or at a assigned location.
/// </summary>
public class SpawnArrow : MonoBehaviour
{
    public GameObject arrowPrefab;
    public GameObject[] locations;
    public int location;

    /// <summary>
    /// Spawns an arrow above the button if there are no given locations, else
    /// put the arrow at the assigned location.
    /// </summary>
    void Spawn()
    {
        if (locations.Length == 0)
            Instantiate(arrowPrefab, transform.position + new Vector3(0, 3, 1), Quaternion.identity);
        else
            Instantiate(arrowPrefab, locations[location].transform.position, transform.rotation);

    }

    /// <summary>
    /// Spawns the arrow when the button is collided by trigger.
    /// </summary>
    /// <param name="other">The other game object colliding with the button.</param>
    private void OnTriggerEnter(Collider other)
    {
        Spawn();
    }
}
