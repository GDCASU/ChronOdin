using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDoor : SimpleTimeManipulation
{
    private void OnCollisionStay(Collision collision)
    {
        // TODO: Sound Effect
        // Active trapdoor if not frozen
        if(timescale != 0)
            if(collision.gameObject.CompareTag("Player"))
            {
                // Door is basically "crumbling"
                gameObject.GetComponent<MeshRenderer>().enabled = false;
                gameObject.GetComponent<Collider>().enabled = false;
            }
    }
}
