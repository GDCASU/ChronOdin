using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets an object to dissapear when the play touches it.
/// 
/// Author: Alben Trang
/// </summary>
public class DisappearWhenPlayerTouch : MonoBehaviour
{
    private bool canDisappear;

    /// <summary>
    /// Makes the object disappear if canDisappear is true and the object it's colliding with has the "Player" tag.
    /// </summary>
    /// <param name="collision">The other object colliding with this one.</param>
    private void OnCollisionEnter(Collision collision)
    {
        if (canDisappear && collision.gameObject.CompareTag("Player"))
        {
            this.gameObject.GetComponent<MeshRenderer>().enabled = false;
            this.gameObject.GetComponent<Collider>().enabled = false;
        }
    }

    /// <summary>
    /// Change if the object can or cannot disappear if the player touches it.
    /// </summary>
    /// <param name="boolean">Boolean value that's true or false.</param>
    public void SetDisappearTrigger(bool boolean)
    {
        canDisappear = boolean;
    }
}
