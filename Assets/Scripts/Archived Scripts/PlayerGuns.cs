using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player keeps all of the guns here.
/// </summary>
public class PlayerGuns : MonoBehaviour
{
    public GameObject[] gunsArray;
    public Transform gunFireLocation;
    public float pickUpRange = 5.0f;

    private int currentGunIndex;

    #region Singleton for PlayerGuns

    public static PlayerGuns instance;

    /// <summary>
    /// Gets the singleton of player guns to call its public functions.
    /// </summary>
    public static PlayerGuns Instance
    {
        get
        {
            if (instance == null)
                Debug.LogError("The PlayerGuns instance is NULL.");

            return instance;
        }
    }

    /// <summary>
    /// Starts the class to set the singleton of it as this.
    /// </summary>
    private void Awake()
    {
        instance = this;
    }
    #endregion

    /// <summary>
    /// Start at frame one by having the player carry the first gun and
    /// deactivate the rest for now.
    /// </summary>
    void Start()
    {
        currentGunIndex = 0;
        for (int i = 1; i < gunsArray.Length; i++)
            gunsArray[i].gameObject.SetActive(false);
    }

    /// <summary>
    /// Check every frame if the player presses the Fire2 button to switch weapons.
    /// Also check every frame to get a pickup object by using raycasts.
    /// </summary>
    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
            SwitchGuns();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (Physics.Raycast(gunFireLocation.position, gunFireLocation.forward, out RaycastHit hit, pickUpRange))
            {
                if (hit.collider.gameObject.CompareTag("Pickup"))
                {
                    RefillGuns();
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// Refill all the player's gun's ammo.
    /// </summary>
    public void RefillGuns()
    {
        foreach (GameObject gun in gunsArray)
            gun.GetComponent<Gun>().RefillAmmo();

        GameManager.Instance.UpdateGunStats();
    }

    /// <summary>
    /// Returns the location for the bullets to spawn.
    /// </summary>
    /// <returns>The transform/location for bullets to spawn.</returns>
    public Transform GetGunFireLocation()
    {
        return gunFireLocation;
    }

    /// <summary>
    /// Returns the stats of the current gun being used.
    /// </summary>
    /// <returns>The name, shot type, and current ammo of the current gun.</returns>
    public (string, string, int) GetCurrentGunStats()
    {
        return gunsArray[currentGunIndex].GetComponent<Gun>().GetGunStats();
    }

    /// <summary>
    /// Switches the gun the player will use.
    /// </summary>
    private void SwitchGuns()
    {
        gunsArray[currentGunIndex].SetActive(false);
        if (currentGunIndex + 1 < gunsArray.Length)
        {
            gunsArray[++currentGunIndex].SetActive(true);
        }
        else
        {
            currentGunIndex = 0;
            gunsArray[currentGunIndex].SetActive(true);
        }
        GameManager.Instance.UpdateGunStats();
    }
}
