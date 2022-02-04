using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object has this script attached so that when the
/// player touches the object, the player goes to a
/// level selected by this script.
/// </summary>
public class FinishPoint : MonoBehaviour
{
    [Tooltip("Levels are from the LevelManager script")]
    public Levels levelSelected;

    /// <summary>
    /// When the player touches the object with this script, they will go to
    /// the level selected here.
    /// </summary>
    /// <param name="collision">The player object colliding with this one</param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            LevelManager.Instance.PickLevel(levelSelected);
        }
    }
}
