using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderSpawner : SimpleTimeManipulation
{
    [SerializeField]
    private float minSpawnSeconds = 3.0f;
    [SerializeField]
    private float maxSpawnSeconds = 6.0f;
    [SerializeField]
    private float forceAppliedToBoulder = 500.0f;
    [SerializeField]
    GameObject[] boulderTypes;

    private float timer = 0f;
    private float determinedSpawnTimer;

    // Start is called before the first frame update
    private void Start()
    {
        UpdateWithGlobalTimescale(MasterTime.singleton.timeScale);
        determinedSpawnTimer = (int)Random.Range(minSpawnSeconds, maxSpawnSeconds);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime * timeScale;

        if (timer >= determinedSpawnTimer)
        {
            SpawnBoulder();
            determinedSpawnTimer = (int)Random.Range(minSpawnSeconds, maxSpawnSeconds);
            timer = 0f;
        }
    }

    private void SpawnBoulder()
    {
        // which type of boulder to spawn
        int boulderChoice = Random.Range(0, boulderTypes.Length);

        GameObject boulder = Instantiate(boulderTypes[boulderChoice], transform.position, transform.rotation);

        boulder.GetComponent<Rigidbody>().AddForce(transform.forward * forceAppliedToBoulder);
    }
}
