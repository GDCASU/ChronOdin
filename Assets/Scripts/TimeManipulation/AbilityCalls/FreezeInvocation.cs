/*
 * Freezes objects for a specified amount of time.
 * The Player has the option to freeze a single object or the environment.
 * To freeze a single object, the Player must look at the object and press the single object freeze button, which instantly consumes a chunk of stamina.
 * To freeze the environment, the Player must press the environment freeze button, which drains stamina until the Player presses the button again or stamina runs out.
 * 
 * Author: Cristion Dominguez
 * Date: 10 September 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FreezeInvocation : MonoBehaviour
{
    [Header("Quantities")]
    [SerializeField, Tooltip("The time the object shall be frozen for")]
    private float _singleFreezeTime = 3f;
    public float SingleFreezeTime { get => _singleFreezeTime; }

    [SerializeField, Tooltip("Chunk of stamina consumed upon freezing a single object")]
    private float singleFreezeStaminaCost = 1f;
    public float SingleFreezeStaminaCost { get => singleFreezeStaminaCost; }

    [SerializeField, Tooltip("Stamina drained per second after freezing the environment")]
    private float environmentFreezeStaminaRate = 1f;

    public static Action<TimeEffect, float, float, bool> freezeAllComplexObjects;  // event container for freezing every complex time object

    private WaitForFixedUpdate waitForFixedUpdate;  // coroutine suspension

    public static FreezeInvocation singleton;

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
    public void SimpleObjectFreeze(SimpleTimeManipulation simpleObject)
    {
        if (TimeStamina.singleton.ConsumeChunk(singleFreezeStaminaCost))
        {
            simpleObject.ActivateSingleObjectEffect(_singleFreezeTime, TimeEffect.Freeze);
            AbilityVisualInvocation.singleton.PlaySingleVFX();
        }
    }
    public void ComplexObjectFreeze(ComplexTimeHub complexObject)
    {
        if (complexObject.transform.GetComponent<ComplexFreeze>() == null)
            return;

        if (TimeStamina.singleton.ConsumeChunk(singleFreezeStaminaCost))
        {
            complexObject.AffectObject(TimeEffect.Freeze, _singleFreezeTime, 0f, true);
            AbilityVisualInvocation.singleton.PlaySingleVFX();
        }
            
    }
    public void EnvironmentFreeze()
    {
        // If the environment is undergoing a time effect that is not environment freeze, do not attempt to freeze the environment.
        if (MasterTime.singleton.timeScale != 1f && MasterTime.singleton.timeScale != 0f)
            return;

        // If the environment freeze ability is toggled off and the Player has stamina, freeze the environment and commence draining stamina.
        if (!AbilityManager.singleton.environmentEffectActive)
        {
            AbilityManager.singleton.ToggleEnvironment(true);

            if (TimeStamina.singleton.CommenceDraining(environmentFreezeStaminaRate))
            {
                MasterTime.singleton.UpdateTime((int)TimeEffect.Freeze);
                freezeAllComplexObjects?.Invoke(TimeEffect.Freeze, TimeStamina.singleton.RemainingDrainTime, 0, false);
                AbilityVisualInvocation.singleton.PlayerEnvironmentVFX();
                StartCoroutine(TrackEnvironmentFreeze());
            }
        }
        // If the environment freeze ability is toggled on, then unfreeze the environment.
        else AbilityManager.singleton.ToggleEnvironment(false);
        
    }
    /// <summary>
    /// Constantly checks that the conditions for environment freeze are satisfied.
    /// If the Player toggled off environment freeze or has no more stamina, then the environment is unfrozen and stamina is no longer draining.
    /// </summary>
    private IEnumerator TrackEnvironmentFreeze()
    {
        while (AbilityManager.singleton.environmentEffectActive && TimeStamina.singleton.Stamina > 0f)
            yield return waitForFixedUpdate;

        MasterTime.singleton.UpdateTime((int)TimeEffect.None);
        freezeAllComplexObjects?.Invoke(TimeEffect.None, 0f, 1f, false);

        // Halt draining.
        TimeStamina.singleton.HaltDraining();
    }
}
