using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TimeEffect
{
    None,
    Freeze,
    Reverse,
    Slow
}

public class ComplexTimeManipulation : MonoBehaviour
{
    private ComplexFreeze objectToFreeze;
    private ComplexReverse objectToReverse;
    private ComplexSlow objectToSlow;

    public bool NewEffectIntroduced { get; private set; }

    public TimeEffect CurrentEffect { get; private set; }
    public float CurrentActiveTime { get; private set; }
    public float CurrentTimescale { get; private set; }
    public float[] CurrentData { get; private set; }

    public TimeEffect IncomingEffect { get; private set; }
    public float IncomingActiveTime { get; private set; }
    public float IncomingTimescale { get; private set; }
    public float[] IncomingData { get; private set; }

    private void Awake()
    {
        objectToFreeze = transform.GetComponent<ComplexFreeze>();
        objectToReverse = transform.GetComponent<ComplexReverse>();
        objectToSlow = transform.GetComponent<ComplexSlow>();

        FreezeInvocation.freezeAllComplexObjects += AffectEntity;
        SlowInvocation.slowAllComplexObjects += AffectEntity;

        NewEffectIntroduced = false;

        CurrentEffect = TimeEffect.None;
        CurrentActiveTime = 0f;
        CurrentTimescale = 1f;
        CurrentData = null;

        IncomingEffect = TimeEffect.None;
        IncomingActiveTime = 0f;
        IncomingTimescale = 1f;
        IncomingData = null;
    }

    public void AffectEntity(TimeEffect effect, float activeTime, float timescale)
    {
        Debug.Log("Hi");

        IncomingEffect = effect;
        IncomingActiveTime = activeTime;
        IncomingTimescale = timescale;

        if (IncomingEffect == TimeEffect.Freeze && objectToFreeze != null)
        {
            IncomingData = objectToFreeze.GetData();
        }
        else if (IncomingEffect == TimeEffect.Reverse && objectToReverse != null)
        {
            IncomingData = objectToReverse.GetData();
        }
        else if (IncomingEffect == TimeEffect.Slow && objectToSlow != null)
        {
            IncomingData = objectToSlow.GetData();
        }

        if (CurrentEffect == TimeEffect.None)
        {
            TransitionToEffect();
        }
        else
        {
            NewEffectIntroduced = true;
        }
    }

    public void TransitionToEffect()
    {
        NewEffectIntroduced = false;

        CurrentEffect = IncomingEffect;
        CurrentActiveTime = IncomingActiveTime;
        CurrentTimescale = IncomingTimescale;
        CurrentData = IncomingData;

        IncomingEffect = TimeEffect.None;
        IncomingActiveTime = 0f;
        IncomingTimescale = 1f;
        IncomingData = null;

        if (CurrentEffect == TimeEffect.Freeze && objectToFreeze != null)
        {
            CurrentData = objectToFreeze.GetData();
            objectToFreeze.Freeze(CurrentActiveTime);
        }
        else if (CurrentEffect == TimeEffect.Reverse && objectToReverse != null)
        {
            CurrentData = objectToReverse.GetData();
            objectToReverse.Reverse(CurrentActiveTime);
        }
        else if (CurrentEffect == TimeEffect.Slow && objectToSlow != null)
        {
            CurrentData = objectToSlow.GetData();
            objectToSlow.Slow(CurrentActiveTime, CurrentTimescale);
        }
    }
}
