using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapProjectile : MonoBehaviour
{
    [SerializeField, Tooltip("")]
    private float speed;

    [SerializeField, Tooltip("")]
    private float lifespan;

    private ShootingTrap originTrap = null;

    private void SetOriginTrap(ShootingTrap originTrap)
    {
        this.originTrap = originTrap;
    }

    private IEnumerator Shoot()
    {
        yield return null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this);
    }
}
