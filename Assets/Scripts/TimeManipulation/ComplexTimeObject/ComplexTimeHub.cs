/*
 * Facilitates the calling of and communication between freeze, reverse, and slow scripts for a complex gameobject.
 * A complex gameobject can not be frozen, reversed, nor slowed by 1 value such as timescale (i.e. a physics-bound cube that saves points in time to reverse to).
 * 
 * Author: Cristion Dominguez
 * Date: 21 November 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Time-related effects an object can experience.
/// </summary>
public enum TimeEffect
{
    None,
    Freeze,
    Reverse,
    Slow
}

public class ComplexTimeHub : MonoBehaviour
{
    // Time mechanic scripts
    private ComplexFreeze objectToFreeze;
    private ComplexReverse objectToReverse;
    private ComplexSlow objectToSlow;

    /// <summary>
    /// Event invoked when a transition to a new time effect occurs.
    /// </summary>
    public Action broadcastTransition;

    /// <summary>
    /// Is a new time effect being introduced?
    /// </summary>
    public bool IntroducingNewEffect { get; private set; }

    private bool interruptingAnEffect = false;  // Is the new time effect interrupting another effect?

    // Fields for the current time effect
    /// <summary>
    /// Time effect currently active.
    /// </summary>
    public TimeEffect CurrentEffect { get; private set; }
    /// <summary>
    /// How long the current effect shall last.
    /// </summary>
    public float CurrentActiveTime { get; private set; }
    /// <summary>
    /// Timescale of the current effect; relevant for reverse and slow effects.
    /// I.E. a timescale of 0.3 shall slow an object more than a timescale of 0.5.
    /// </summary>
    public float CurrentTimescale { get; private set; }
    /// <summary>
    /// Data of the current effect retrieved from its corresponding script.
    /// </summary>
    public float[] CurrentData { get; private set; }

    // Field for the previous time effect
    /// <summary>
    /// Time effect previously active.
    /// </summary>
    public TimeEffect PreviousEffect { get; private set; }
    /// <summary>
    /// How long the previous effect shall last.
    /// </summary>
    public float PreviousActiveTime { get; private set; }
    /// <summary>
    /// Timescale of the previous effect; relevant for reverse and slow effects.
    /// I.E. a timescale of 0.3 shall slow an object more than a timescale of 0.5.
    /// </summary>
    public float PreviousTimescale { get; private set; }
    /// <summary>
    /// Data of the previous effect retrieved from its corresponding script.
    /// </summary>
    public float[] PreviousData { get; private set; }

    /// <summary>
    /// Acquires time mechanic scripts, subscribes the AffectEntity method to the freeze environment and slow environment events, and sets both the
    /// current and previous effects.
    /// </summary>
    private void Awake()
    {
        objectToFreeze = transform.GetComponent<ComplexFreeze>();
        objectToReverse = transform.GetComponent<ComplexReverse>();
        objectToSlow = transform.GetComponent<ComplexSlow>();

        FreezeInvocation.freezeAllComplexObjects += AffectObject;
        SlowInvocation.slowAllComplexObjects += AffectObject;

        IntroducingNewEffect = false;

        CurrentEffect = TimeEffect.None;
        CurrentActiveTime = 0f;
        CurrentTimescale = 1f;
        CurrentData = null;

        PreviousEffect = TimeEffect.None;
        PreviousActiveTime = 0f;
        PreviousTimescale = 1f;
        PreviousData = null;
    }

    /// <summary>
    /// Saves the current effect as a previous effect and modifies the current effect, and transitions to the new effect.
    /// If the new effect does not interrupt an active time effect (TimeEffect.None), then the transition is handled by this method.
    /// If the new effect does not interrupt an active time effect, then the transition is handled by the script corresponding to the previous effect.
    /// For example, if an object is reversing whilst it receives an order to slow in forward time, then a ComplexReverse child must invoke the TransitionToNextEffect() method.
    /// </summary>
    /// <param name="effect"> the new time effect the gameobject shall experience </param>
    /// <param name="activeTime"> how long the effect shall last </param>
    /// <param name="timescale"> the timescale of the effect (relevant for reverse and slow) </param>
    public void AffectObject(TimeEffect effect, float activeTime, float timescale)
    {
        // If the script for the corresponding effect does not exist, do nothing.
        if (effect == TimeEffect.Freeze && objectToFreeze == null)
        {
            return;
        }
        else if (effect == TimeEffect.Reverse && objectToReverse == null)
        {
            return;
        }
        else if (effect == TimeEffect.Slow && objectToSlow == null)
        {
            return;
        }

        // Determine if the new effect is interrupting an active effect.
        if (CurrentEffect == TimeEffect.None)
        {
            interruptingAnEffect = false;
        }
        else
        {
            interruptingAnEffect = true;
        }

        // Save the current effect.
        PreviousEffect = CurrentEffect;
        PreviousActiveTime = CurrentActiveTime;
        PreviousTimescale = CurrentTimescale;
        PreviousTimescale = CurrentTimescale;

        // Modify the current effect.
        CurrentEffect = effect;
        CurrentActiveTime = activeTime;
        CurrentTimescale = timescale;
        if (CurrentEffect == TimeEffect.Freeze)
        {
            CurrentData = objectToFreeze.GetData();
        }
        else if (CurrentEffect == TimeEffect.Reverse)
        {
            CurrentData = objectToReverse.GetData();
        }
        else if (CurrentEffect == TimeEffect.Slow)
        {
            CurrentData = objectToSlow.GetData();
        }

        // Indicate that a new effect was introduced and if the new effect was not introduced whilst another effect was occurring, transition to the new effect.
        IntroducingNewEffect = true;
        if (interruptingAnEffect == false)
        {
            TransitionToNextEffect();
        }
    }

    /// <summary>
    /// Transitions to the next time effect. If a new effect was not introduced, then the next effect is the object moving normally with time.
    /// Invokes broadcastTransition event.
    /// </summary>
    public void TransitionToNextEffect()
    {
        // If a new effect was not introduced, then reset the current effect.
        if (IntroducingNewEffect == false)
        {
            CurrentEffect = TimeEffect.None;
            CurrentTimescale = 1f;
            CurrentActiveTime = 0f;
            CurrentData = null;
        }

        // Indicate that the next effect has been transitioned to and broadcast this transition.
        IntroducingNewEffect = false;
        broadcastTransition?.Invoke();

        // Apply the current time effect to the gameobject.
        if (CurrentEffect == TimeEffect.Freeze && objectToFreeze != null)
        {
            objectToFreeze.Freeze(CurrentActiveTime);
        }
        else if (CurrentEffect == TimeEffect.Reverse && objectToReverse != null)
        {
            objectToReverse.Reverse(CurrentActiveTime);
        }
        else if (CurrentEffect == TimeEffect.Slow && objectToSlow != null)
        {
            objectToSlow.Slow(CurrentActiveTime, CurrentTimescale);
        }
    }
}
