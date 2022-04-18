/*
 * Provides manipulation of stamina bar in Player's canvas.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    private Slider staminaBar;  // bar in Player's canvas
    public static StaminaBar singleton;

    /// <summary>
    /// Sets up singleton.
    /// </summary>
    private void Awake()
    {
        if (singleton == null)
            singleton = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Acquires the stamina bar and sets its values.
    /// </summary>
    private void Start()
    {
        staminaBar = GetComponent<Slider>();
        staminaBar.maxValue = TimeStamina.singleton.MaxStamina;
        staminaBar.value = staminaBar.maxValue;
    }

    /// <summary>
    /// Alters the stamina bar based on the received stamina value.
    /// </summary>
    /// <param name="stamina"> value to be represented by the stamina bar </param>
    public void SetStamina(float stamina)
    {
        if (stamina >= 0)
            staminaBar.value = stamina;
        else
            staminaBar.value = 0;
    }
}
