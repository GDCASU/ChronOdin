using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField] GameObject cubePrefab;
    private bool shooting = false;
    [SerializeField] private float bulletSpeed = 45f;
    private float nextTimeToShoot = 0f;
    private float fireRate = 0.5f;
    public float reloadTime = 8f;
    [SerializeField] int fireMode = 0;

    private void Update()
    {
        if (Time.time >= nextTimeToShoot)
        {
            //cooldown management
            nextTimeToShoot = Time.time + 1f / fireRate;
            Shoot();
        }
    }
    private void Shoot()
    {
        shooting = true;
        GameObject b = Instantiate(cubePrefab, transform.position, transform.rotation) as GameObject;
        b.transform.position = transform.position;
        Vector3 difference = (transform.forward - b.transform.position).normalized;
        b.transform.rotation = transform.rotation;
        b.GetComponent<Rigidbody>().AddForce(difference * bulletSpeed, ForceMode.Impulse);
        if(fireMode == 1)
        {
            b.GetComponent<SlowDownSpeedUpObject>().SlowDown();
        }
        if(fireMode == 2)
        {
            b.GetComponent<SlowDownSpeedUpObject>().SpeedUp();
        }
        StartCoroutine(Reload());
    }
    IEnumerator Reload()
    {
        yield return new WaitForSeconds(reloadTime);
        shooting = false;
    }
}
