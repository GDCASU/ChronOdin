using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTargetAbilityTracker : MonoBehaviour
{
    public static SimpleTargetAbilityTracker singleton;

    private static FreezeInvocation freezeAbility;
    private static ReverseInvocation reverseAbility;
    private static SlowInvocation slowAbility;

    private static SimpleTimeManipulation frozenObject;
    private static SimpleTimeManipulation reversingObject;

    private static Coroutine frozenCoroutine;
    private static Coroutine reversingCoroutine;

    private bool frozenObjectNotSubscribed = false;
    private bool reversingObjectNotSubscribed = false;

    private void Awake()
    {
        if (singleton == null) singleton = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        Transform player = PlayerController.singleton.transform;

        freezeAbility = player.GetComponent<FreezeInvocation>();
        reverseAbility = player.GetComponent<ReverseInvocation>();
        slowAbility = player.GetComponent<SlowInvocation>();
    }

    public void SetFrozenObject(SimpleTimeManipulation simpleObject, Coroutine simpleCoroutine)
    {
        frozenObject = simpleObject;
        frozenCoroutine = simpleCoroutine;

        if (reversingObject != null && frozenObject.transform.Equals(reversingObject.transform))
        {
            ResetReversingObject();
        }

        if (MasterTime.singleton.timeScale != 1f)
        {
            frozenObjectNotSubscribed = true;
            MasterTime.singleton.updateTimeScaleEvent -= frozenObject.UpdateTimeScale;
        }
    }

    public void ResetFrozenObject()
    {
        if (frozenObjectNotSubscribed)
        {
            frozenObjectNotSubscribed = false;
            MasterTime.singleton.updateTimeScaleEvent += frozenObject.UpdateTimeScale;
        }

        frozenObject = null;
        StopCoroutine(frozenCoroutine);
        frozenCoroutine = null;
    }

    public void SetReversingObject(SimpleTimeManipulation simpleObject, Coroutine simpleCoroutine)
    {
        reversingObject = simpleObject;
        reversingCoroutine = simpleCoroutine;

        if (frozenObject != null && reversingObject.transform.Equals(frozenObject.transform))
        {
            ResetFrozenObject();
        }

        if (MasterTime.singleton.timeScale != 1f)
        {
            reversingObjectNotSubscribed = true;
            MasterTime.singleton.updateTimeScaleEvent -= reversingObject.UpdateTimeScale;
        }
    }

    public void ResetReversingObject()
    {
        if (reversingObjectNotSubscribed)
        {
            reversingObjectNotSubscribed = false;
            MasterTime.singleton.updateTimeScaleEvent += reversingObject.UpdateTimeScale;
        }

        reversingObject = null;
        StopCoroutine(reversingCoroutine);
        reversingCoroutine = null;
    }

    public void ResetObjects()
    {
        //StopCoroutine(frozenCoroutine);
        //StopCoroutine(reversingCoroutine);

        ResetFrozenObject();
        ResetReversingObject();
    }
}
