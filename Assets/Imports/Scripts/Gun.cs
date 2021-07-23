using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gun class that holds ammo, fire the ammo, and can have it refill ammo.
/// </summary>
public class Gun : MonoBehaviour
{
    #region Variables

    public enum ShotTypes { Single, Continuous };

    [Header("Essentials")]
    public GameObject bulletPrefab;

    [Header("Stats")]
    public ShotTypes shotType;
    [Range(10, 30)] public int maxAmmo = 10;
    [Range(1, 10)] public int damagePerBullet = 1;
    [Range(100, 500)] public int shotSpeed = 10;
    [Range(0.1f, 1.0f)] public float continuousFireRate = 0.2f;

    private Transform gunFireLocation;
    private List<GameObject> bullets;
    private int ammo;
    private bool readyContinousFire;
    #endregion

    /// <summary>
    /// Start at frame one for the gun to have its ammo at the assigned max capacity.
    /// Then create an object pool full of bullets at max capacity.
    /// </summary>
    private void Start()
    {
        gunFireLocation = PlayerGuns.Instance.GetGunFireLocation();
        bullets = new List<GameObject>();           // Object pool for each gun's bullets
        readyContinousFire = true;
        RefillAmmo();
        for (int i = 0; i < maxAmmo + 20; i++)      // Add 20 extra bullets in the pool
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bullets.Add(bullet);
        }

        GameManager.Instance.UpdateGunStats();
    }

    /// <summary>
    /// Check every frame for player input.
    /// </summary>
    private void Update()
    {
        if (shotType == ShotTypes.Single && Input.GetButtonDown("Fire1"))
            FireGun();
        else if (shotType == ShotTypes.Continuous && Input.GetButton("Fire1") && readyContinousFire)
        {
            FireGun();
            readyContinousFire = false;
            StartCoroutine(FireRate(continuousFireRate));
        }
    }

    /// <summary>
    /// Set the ammo back to its maximum capacity.
    /// </summary>
    public void RefillAmmo() => ammo = maxAmmo;

    /// <summary>
    /// Returns a tuple containing the stats of the given gun.
    /// </summary>
    /// <returns>The name, shot type, and current ammo.</returns>
    public (string, string, int) GetGunStats()
    {
        return (this.gameObject.name, shotType.ToString(), ammo);
    }

    /// <summary>
    /// Fires a bullet from the assigned location.
    /// </summary>
    public void FireGun()
    {
        if (ammo > 0)
        {
            GameObject currentBullet = null;
            foreach (GameObject bullet in bullets)
            {
                if (!bullet.activeInHierarchy)
                {
                    bullet.SetActive(true);
                    currentBullet = bullet;
                    break;
                }
            }

            currentBullet.GetComponent<Bullet>().BulletDamage = damagePerBullet;
            currentBullet.GetComponent<Bullet>().FireThenDeactivate();
            currentBullet.transform.position = gunFireLocation.position;
            currentBullet.transform.rotation = gunFireLocation.rotation;
            currentBullet.GetComponent<Rigidbody>().AddForce(currentBullet.transform.forward * shotSpeed);
            ammo--;
            GameManager.Instance.UpdateGunStats();
        }
    }
    
    /// <summary>
    /// Sets some number of time before firing the next continuous bullet.
    /// </summary>
    /// <param name="fireRate">Time it takes before the gun is ready to fire.</param>
    /// <returns>Yields time before each bullet at continuous fire.</returns>
    private IEnumerator FireRate(float fireRate)
    {
        yield return new WaitForSeconds(fireRate);
        readyContinousFire = true;
    }
}
