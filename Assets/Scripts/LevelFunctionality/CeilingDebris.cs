using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingDebris : SimpleTimeManipulation
{
    [SerializeField]
    private float minSpawnSeconds = 1.0f;
    [SerializeField]
    private float maxSpawnSeconds = 4.0f;
    [SerializeField]
    private int minNumDebris = 1;
    [SerializeField]
    private int maxNumDebris = 3;
    [SerializeField]
    private float minStaggerSeconds = 0.2f;
    [SerializeField]
    private float maxStaggerSeconds = 0.8f;

    [SerializeField]
    private GameObject[] debrisToSpawn;

    private float timer = 0f;
    private float determinedSpawnTimer;
    private float spawnZoneLength;
    private float spawnZoneWidth;

    private void Start()
    {
        UpdateTimescale(MasterTime.singleton.timeScale);
        spawnZoneLength = transform.localScale.x / 2;
        spawnZoneWidth = transform.localScale.z / 2;
        determinedSpawnTimer = (int)Random.Range(minSpawnSeconds, maxSpawnSeconds);
    }

    // Update is called once per frame
    private void Update()
    {
        timer += Time.deltaTime * timeScale;

        if(timer >= determinedSpawnTimer)
        {
            StartCoroutine(SpawnDebris());
            determinedSpawnTimer = (int)Random.Range(minSpawnSeconds, maxSpawnSeconds);
            timer = 0f;
        }
    }

    // TODO: Decide how to remove debris over time. Possibly destroy on collision
    private IEnumerator SpawnDebris()
    {
        // number of debris to spawn
        int numDebris = Random.Range(minNumDebris, maxNumDebris + 1);

        for(int i = 0; i < numDebris; i++)
        {
            // random position within spawner
            float randX = transform.position.x + Random.Range(-spawnZoneLength, spawnZoneLength);
            float randZ = transform.position.z + Random.Range(-spawnZoneWidth, spawnZoneWidth);
            Vector3 spawnpoint = new Vector3(randX, transform.position.y, randZ);

            // which type of debris to spawn
            int debrisChoice = Random.Range(0, debrisToSpawn.Length);

            Instantiate(debrisToSpawn[debrisChoice], spawnpoint, Quaternion.identity);

            //stagger spawning of debris
            yield return new WaitForSeconds(Random.Range(minStaggerSeconds, maxStaggerSeconds));
        }
    }
}
