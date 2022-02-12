using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDoor : SimpleTimeManipulation
{
    /// <summary>
    /// Updates the local timeScale variable before the first update is called
    /// </summary>
    private void Start() => UpdateWithGlobalTimescale(MasterTime.singleton.timeScale);
    private void OnCollisionStay(Collision collision)
    {
        // TODO: Sound Effect
        // Active trapdoor if not frozen
        if(timeScale != 0)
            if(collision.gameObject.CompareTag("Player"))
            {
                // Door is basically "crumbling"
                gameObject.GetComponent<MeshRenderer>().enabled = false;
                gameObject.GetComponent<Collider>().enabled = false;
            }
    }
}
