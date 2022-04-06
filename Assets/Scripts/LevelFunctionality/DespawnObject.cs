using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnObject : MonoBehaviour
{
    public bool startTimerOnSpawn = true;
    public float lifeTime = 7;
    void Start()
    {
        if (startTimerOnSpawn) StartDespawnTimer();
    }

    public void StartDespawnTimer() => StartCoroutine(Despawn());
    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
