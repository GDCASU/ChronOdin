using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStamina : MonoBehaviour
{
    [SerializeField]
    private float maxStamina = 3f;

    [SerializeField]
    private float chunkAmount = 1f;

    [SerializeField]
    private float drainRate = .3f / 9f;

    [SerializeField]
    private float regenRate = 0.04f;

    [SerializeField]
    private float regenCooldown = 2f;

    public float Stamina { get; private set; }
    public float RemainingDrainTime
    {
        get => Stamina * drainRate * Time.fixedDeltaTime;
    }

    private bool isDraining = false;
    private float regenTimer = 0f;

    public static TimeStamina singleton;
    private WaitForFixedUpdate waitForFixedUpdate;

    private void Awake()
    {
        if (singleton == null)
            singleton = this;
        else
            Destroy(gameObject);

        waitForFixedUpdate = new WaitForFixedUpdate();

        Stamina = maxStamina;
    }

    private void FixedUpdate()
    {
        if (!isDraining && Stamina < maxStamina)
        {
            if (regenTimer < regenCooldown)
            {
                regenTimer += Time.deltaTime;
            }
            else
            {
                Stamina += regenRate;

                if (Stamina > maxStamina)
                {
                    Stamina = maxStamina;
                }
            }
        }
    }

    public bool ConsumeChunk()
    {
        if (Stamina <= 0)
            return false;

        Stamina -= chunkAmount;
        
        if (Stamina < 0)
            Stamina = 0;

        regenTimer = 0f;

        return true;
    }

    public bool CommenceDraining()
    {
        if (Stamina <= 0 && isDraining)
            return false;

        isDraining = true;
        StartCoroutine(Drain());

        return true;
    }
    private IEnumerator Drain()
    {
        while (isDraining && Stamina > 0)
        {
            Stamina -= drainRate;
            yield return waitForFixedUpdate;
        }

        isDraining = false;
    }
    public void HaltDraining()
    {
        isDraining = false;

        regenTimer = 0f;
    }
}
