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

    public TimeEffect CurrentEffect { get; private set; }
    public float CurrentTimescale { get; private set; }
    public float[] CurrentData { get; private set; }
    public TimeEffect NewEffect { get; private set; }
    public float NewTimescale { get; private set; }
    public float[] NewData { get; private set; }

    private void Awake()
    {
        objectToFreeze = transform.GetComponent<ComplexFreeze>();
        objectToReverse = transform.GetComponent<ComplexReverse>();
        objectToSlow = transform.GetComponent<ComplexSlow>();

        FreezeInvocation.freezeAllComplexObjects += AffectEntity;
        SlowInvocation.slowAllComplexObjects += AffectEntity;

        CurrentEffect = TimeEffect.None;
        CurrentTimescale = 1f;

        NewEffect = TimeEffect.None;
        NewTimescale = 1f;
    }

    public void AffectEntity(TimeEffect effect, float activeTime, float timescale)
    {
        if (CurrentEffect != TimeEffect.None)
        {
            NewEffect = effect;
            NewTimescale = timescale;

            if (CurrentEffect == TimeEffect.Freeze && objectToFreeze != null)
            {
                objectToFreeze.GetData();
            }
            else if (CurrentEffect == TimeEffect.Reverse && objectToReverse != null)
            {
                objectToReverse.GetData();
            }
            else if (CurrentEffect == TimeEffect.Slow && objectToSlow != null)
            {
                objectToSlow.GetData();
            }

            while (CurrentEffect != TimeEffect.None) ;

            NewEffect = TimeEffect.None;
            NewTimescale = 1f;
            NewData = null;
        }

        CurrentEffect = effect;
        CurrentTimescale = timescale;

        if (CurrentEffect == TimeEffect.Freeze && objectToFreeze != null)
        {
            objectToFreeze.Freeze(activeTime);
        }
        else if (CurrentEffect == TimeEffect.Reverse && objectToReverse != null)
        {
            objectToReverse.Reverse(activeTime);
        }
        else if (CurrentEffect == TimeEffect.Slow && objectToSlow != null)
        {
            objectToSlow.Slow(activeTime, timescale);
        }
        else
        {
            ResetCurrentTimeEffect();
        }
    }

    public void SetCurrentEffectData(float[] effectData)
    {
        CurrentData = effectData;
    }

    public void ResetCurrentTimeEffect()
    {
        CurrentEffect = TimeEffect.None;
        CurrentTimescale = 1f;
        CurrentData = null;
    }
}
