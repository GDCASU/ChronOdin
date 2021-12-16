/*
 * A trap that fires projectiles and can only be frozen.
 * 
 * Author: Cristion Dominguez
 * Date: 16 Dec. 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingTrap : SimpleTimeManipulation
{
    [SerializeField, Tooltip("Projectile to fire out.")]
    private GameObject projectilePrefab;

    [SerializeField, Tooltip("Where the projectile spawns.")]
    private Transform projectileSpawnpoint;

    [SerializeField, Tooltip("How frequently the trap fire projectiles.")]
    private float fireRate = 3f;

    private float elapsedTime = 0f;  // time since the last projectile was fired

    /// <summary>
    /// Only permits the trap to be frozen.
    /// </summary>
    /// <param name="newTimescale"></param>
    public override void UpdateTimescale(float newTimescale)
    {
        base.UpdateTimescale(newTimescale);

        if (timescale != 1 && timescale != 0)
        {
            timescale = 1f;
        }
    }

    /// <summary>
    /// Fires a projectile at the specified fire rate.
    /// </summary>
    private void Update()
    {
        if (elapsedTime >= fireRate)
        {
            Instantiate(projectilePrefab, projectileSpawnpoint.position, projectileSpawnpoint.rotation).GetComponent<ShootingProjectile>().launch();
            elapsedTime = 0f;
        }
        elapsedTime += (Time.deltaTime * timescale);
    }
}
