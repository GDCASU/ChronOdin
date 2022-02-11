/**
 * Stores and manipulates the stamina the Player has available to affect time.
 * Also updates the stamina bar belonging to the Player.
 * 
 * Author: Cristion Dominguez
 * Date: 11 February 2022
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStamina : MonoBehaviour
{
    [SerializeField, Tooltip("Max stamina the Player can possess")]
    private float _maxStamina = 3f;
    public float MaxStamina { get => _maxStamina; }

    [SerializeField, Tooltip("Rate at which stamina regenerates per second")]
    private float regenRate = 2f;

    [SerializeField, Tooltip("Time required for stamina to regenerate after expending it")]
    private float regenDelay = 2f;

    // Stamina the Player currently possesses
    public float Stamina { get; private set; }

    // Stamina incremented every fixed update
    private float regenStaminaPerFixedUpdate;
    // Stamina decremented every fixed update
    private float drainStaminPerFixedUpdate;

    // Time until the stamina is depleted when draining
    public float RemainingDrainTime
    {
        get => Stamina * Time.fixedDeltaTime / drainStaminPerFixedUpdate;
    }

    private bool isDraining = false;  // Is the Player's stamina being drained?
    private float timeSinceConsumption = 0f;  // time since stamina was last consumed

    public static TimeStamina singleton;
    private WaitForFixedUpdate waitForFixedUpdate;

    /// <summary>
    /// Prepares the singleton, coroutine suspension, and sets stamina values.
    /// </summary>
    private void Awake()
    {
        if (singleton == null)
            singleton = this;
        else
            Destroy(gameObject);

        waitForFixedUpdate = new WaitForFixedUpdate();

        Stamina = _maxStamina;
        regenStaminaPerFixedUpdate = regenRate * Time.fixedDeltaTime;
    }

    /// <summary>
    /// Updates the stamina regeneration per fixed update when a change in the editor occurs during playtime.
    /// </summary>
    private void OnValidate()
    {
        regenStaminaPerFixedUpdate = regenRate * Time.fixedDeltaTime;
    }

    /// <summary>
    /// Regenerates stamina if it's not being drained, less than max, and was not consumed for the time specified by regen delay.
    /// </summary>
    private void FixedUpdate()
    {
        if (!isDraining && Stamina < _maxStamina)
        {
            if (timeSinceConsumption < regenDelay)
            {
                timeSinceConsumption += Time.deltaTime;
            }
            else
            {
                Stamina += regenStaminaPerFixedUpdate;

                if (Stamina > _maxStamina)
                {
                    Stamina = _maxStamina;
                }

                StaminaBar.singleton.SetStamina(Stamina);
            }
        }
    }

    /// <summary>
    /// Instantly removes a chunk from the current stamina if stamina is greater than 0.
    /// If stamina is greater than 0, true is returned. Otherwise, false is returned.
    /// If the amount is more than the max or less than 0, then stamina is set to 0.
    /// </summary>
    /// <param name="chunkAmount"> amount to remove from current stamina </param>
    /// <returns> Is stamina greater than 0? </returns>
    public bool ConsumeChunk(float chunkAmount)
    {
        if (Stamina <= 0f)
            return false;

        if (chunkAmount < 0f)
        {
            Stamina = 0f;
        }
        else
        {
            Stamina -= chunkAmount;

            if (Stamina < 0f)
                Stamina = 0f;
        }

        StaminaBar.singleton.SetStamina(Stamina);
        timeSinceConsumption = 0f;

        return true;
    }

    /// <summary>
    /// Commences draining stamina based on the provided rate (stamina/second) if stamina is greater than 0.
    /// If the stamina is greater than 0, true is returned. Otherwise, false is returned.
    /// </summary>
    /// <param name="staminaPerSec"> rate at which stamina is drained; stamina decremented per second </param>
    /// <returns> Is stamina greather than 0? </returns>
    public bool CommenceDraining(float staminaPerSec)
    {
        if (Stamina <= 0f && isDraining)
            return false;

        drainStaminPerFixedUpdate = staminaPerSec * Time.fixedDeltaTime;
        isDraining = true;
        StartCoroutine(Drain());

        return true;
    }
    /// <summary>
    /// Drains stamina at the provided rate as long as stamina is greater than 0 and draining is not interrupted from a foreign entity.
    /// </summary>
    private IEnumerator Drain()
    {
        while (Stamina > 0f && isDraining)
        {
            Stamina -= drainStaminPerFixedUpdate;
            StaminaBar.singleton.SetStamina(Stamina);
            yield return waitForFixedUpdate;
        }

        HaltDraining();
    }
    /// <summary>
    /// Halts draining of stamina.
    /// </summary>
    public void HaltDraining()
    {
        drainStaminPerFixedUpdate = 0f;
        isDraining = false;
        timeSinceConsumption = 0f;
    }
}
