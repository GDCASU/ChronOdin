/*
 * Slows objects for a specified amount of time.
 * The Player has the option to slow a single object or the environment.
 * To slow a single object, the Player must look at the object and press the single object slow button, which instantly consumes a chunk of stamina.
 * To slow the environment, the Player must press the environment slow button, which drains stamina until the Player presses the button again or stamina runs out.
 * 
 * Author: Cristion Dominguez
 * Date: 17 September 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SlowInvocation : MonoBehaviour
{
    [Header("Quantities")]
    [SerializeField, Tooltip("The time the object shall be slowed for")]
    private float _singleSlowTime = 3f;
    public float SingleSlowTime { get => _singleSlowTime; }

    [SerializeField, Tooltip("How slow the object shall be")]
    private float slowFactor = 0.5f;

    [SerializeField, Tooltip("Chunk of stamina consumed upon slowing a single object")]
    private float singleSlowStaminaCost = 1f;

    public float SingleSlowStaminaCost { get => singleSlowStaminaCost; }

    [SerializeField, Tooltip("RStamina drained per second after slowing the environment")]
    private float environmentSlowStaminaRate = 1f;

    public static Action<TimeEffect, float, float, bool> slowAllComplexObjects;  // event container for slowing every complex time object

    private WaitForFixedUpdate waitForFixedUpdate;  // coroutine suspension

    public static SlowInvocation singleton;

    /// <summary>
    /// Sets up singleton and initializes the coroutine suspension.
    /// </summary>
    private void Awake()
    {
        if (singleton == null)
            singleton = this;
        else
            Destroy(gameObject);

        waitForFixedUpdate = new WaitForFixedUpdate();
    }
    public void SimpleObjectSlow(SimpleTimeManipulation simpleObject)
    {
        if (TimeStamina.singleton.ConsumeChunk(singleSlowStaminaCost))
            simpleObject.ActivateSingleObjectEffect(_singleSlowTime, TimeEffect.Slow);
    }
    public void ComplexObjectSlow(ComplexTimeHub complexObject)
    {
        if (complexObject.transform.GetComponent<ComplexSlow>() == null)
            return;

        if (TimeStamina.singleton.ConsumeChunk(singleSlowStaminaCost))
            complexObject.AffectObject(TimeEffect.Slow, _singleSlowTime, slowFactor, true);
    }
    public void EnvironmentSlow()
    {
        if (MasterTime.singleton.timeScale > 1f || MasterTime.singleton.timeScale <= 0f)
            return;

        // If the environment slow ability is toggled off and the Player has stamina, freeze the environment and commence draining stamina.
        if (!AbilityManager.singleton.environmentEffectActive)
        {
            if (TimeStamina.singleton.CommenceDraining(environmentSlowStaminaRate))
            {
                MasterTime.singleton.UpdateTime((int)TimeEffect.Slow);
                slowAllComplexObjects?.Invoke(TimeEffect.Slow, TimeStamina.singleton.RemainingDrainTime, slowFactor, false);
                AbilityManager.singleton.ToggleEnvironment(true);
                StartCoroutine(TrackEnvironmentSlow());
            }
        }
        // If the environment slow ability is toggled on, then unslow the environment.
        else AbilityManager.singleton.ToggleEnvironment(false);
        
    }
    /// <summary>
    /// Constantly checks that the conditions for environment slow are satisfied.
    /// If the Player toggled off environment slow or has no more stamina, then the environment is unslowed and stamina is no longer draining.
    /// </summary>
    private IEnumerator TrackEnvironmentSlow()
    {
        while (AbilityManager.singleton.environmentEffectActive && TimeStamina.singleton.Stamina > 0)
            yield return waitForFixedUpdate;

        MasterTime.singleton.UpdateTime((int)TimeEffect.None);
        slowAllComplexObjects?.Invoke(TimeEffect.None, 0f, 1f, false);

        // If the Player toggled off environment slow, then halt draining.
        // Otherwise, toggle off the environment slow automatically.
        if (!AbilityManager.singleton.environmentEffectActive)
        {
            TimeStamina.singleton.HaltDraining();
        }
        else
        {
            AbilityManager.singleton.ToggleEnvironment(false);
        }
    }
}
