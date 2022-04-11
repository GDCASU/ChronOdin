using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDoor : SimpleTimeManipulation
{
    public float delay = 0.75f;
    public List<GameObject> rocks;
    /// <summary>
    /// Updates the local timeScale variable before the first update is called
    /// </summary>
    private void Start() => UpdateWithGlobalTimescale(MasterTime.singleton.timeScale);
    private void OnCollisionEnter(Collision collision)
    {
        // TODO: Sound Effect
        // Active trapdoor if not frozen
        if (timeScale != 0)
            if (collision.gameObject.CompareTag("Player")) StartCoroutine(doorFallDelay());
    }
    IEnumerator doorFallDelay()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        float timer = delay;
        rocks[8].GetComponent<Rigidbody>().useGravity = true;
        while (timer > 0)
        {
            rocks[Random.Range(0, 20)].GetComponent<Rigidbody>().useGravity = true;         
            timer -= .02f * timeScale;
            yield return new WaitForSeconds(.02f);
        }
        transform.DetachChildren();
        foreach (GameObject rock in rocks)
        {
            rock.GetComponent<Rigidbody>().useGravity = true;
            rock.GetComponent<DespawnObject>().StartDespawnTimer();
        }
        gameObject.GetComponent<Collider>().enabled = false;
        Destroy(gameObject);
    }
}
