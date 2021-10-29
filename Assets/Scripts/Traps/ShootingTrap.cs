using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingTrap : MonoBehaviour
{
    [SerializeField, Tooltip("")]
    private GameObject projectile;

    [SerializeField, Tooltip("")]
    private float shootingInterval;

    private bool isFreezable = false;
    private float elapsedTime = 0f;

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= shootingInterval)
        {
            projectile = Instantiate(projectile, transform.position, transform.rotation);
            
        }
    }

    public void MakeFreezable()
    {
        isFreezable = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag.Equals("Player"))
        {
            // MURK THE PLAYER
        }
    }
}
