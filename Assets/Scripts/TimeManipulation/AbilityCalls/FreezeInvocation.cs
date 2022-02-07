/*
 * Freezes objects for a specified amount of time.
 * The Player has the option to freeze a single object or the environment.
 * To freeze a single object, the Player must look at the object and press the freeze single object button, which instantly consumes a chunk of stamina.
 * To freeze the environment, the Player must press the freeze environment button, which drains stamina until the Player presses the button again or stamina runs out.
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
    [Header("Button")]
    [Tooltip("The button to freeze a single object")]
    [SerializeField]
    private KeyCode freezeSingleObjectButton = KeyCode.F;

    [Tooltip("The button to freeze the entire environment")]
    [SerializeField]
    private KeyCode freezeEnvironmentButton = KeyCode.G;

    [Header("Time Values")]
    [Tooltip("The time the object shall be frozen for")]
    [SerializeField]
    private float freezeSingleTime = 5f;

    [Header("Transforms")]
    [Tooltip("The camera providing the Player vision")]
    [SerializeField]
    private Transform playerCamera;

    // The remaining times for active freeze sub-abilities and cooldowns
    [HideInInspector]
    public float RemainingSingleActiveTime { get; private set; }
    [HideInInspector]
    public float RemainingSingleCooldown { get; private set; }
    [HideInInspector]
    public float RemainingEnvironmentCooldown { get; private set; }

    private bool canInitiateSingleFreeze = true;  // Is the single freeze cooldown inactive?
    private bool canInitiateEnvironmentFreeze = true;  // Is the environment freeze cooldown inactive?
    SimpleTimeManipulation simpleObject = null;  // object with a simple freeze mechanism
    ComplexTimeHub complexObject = null;  // objecct with a complex freeze mechanism
    public static Action<TimeEffect, float, float> freezeAllComplexObjects;  // event container for freezing every freezeable object

    // NEW
    private bool environmentActive = false;
    private WaitForFixedUpdate waitForFixedUpdate;

    private void Awake()
    {
        waitForFixedUpdate = new WaitForFixedUpdate();
    }

    /// <summary>
    /// Freezes a single object or the environment depending on Player input and the ability to freeze a specific object(s).
    /// </summary>
    private void Update()
    {
        // If the Player presses the freeze single object button and the corresponding cooldown is inactive, attempt to freeze a single object.
        if (Input.GetKeyDown(freezeSingleObjectButton) && canInitiateSingleFreeze)
        {
            // 
            Transform someObject = PlayerInteractions.singleton.RaycastTransform();

            // 
            simpleObject = someObject.transform.GetComponent<SimpleTimeManipulation>();
            if (simpleObject != null)
            {
                if (TimeStamina.singleton.ConsumeChunk())
                    simpleObject.UpdateTimeScale(0f);

                return;
            }

            complexObject = someObject.transform.GetComponent<ComplexTimeHub>();
            if (complexObject != null)
            {
                // If the object does not possess a freeze script, then do not activate cooldown.
                if (complexObject.transform.GetComponent<ComplexFreeze>() == null)
                {
                    return;
                }

                if (TimeStamina.singleton.ConsumeChunk())
                    complexObject.AffectObject(TimeEffect.Freeze, freezeSingleTime, 0f);

                return;
            }
        }

        // If the Player presses the freeze environment button and the corresponding cooldown is inactive, attempt to freeze all freezeable objects.
        if (Input.GetKeyDown(freezeEnvironmentButton) && canInitiateEnvironmentFreeze)
        {
            if (!environmentActive)
            {
                if (TimeStamina.singleton.CommenceDraining())
                {
                    // If there are freezeable objects existing in the scene, then freeze all of them and activate the freeze environment cooldown.
                    MasterTime.singleton.UpdateTime(0);
                    if (freezeAllComplexObjects != null) freezeAllComplexObjects(TimeEffect.Freeze, TimeStamina.singleton.RemainingDrainTime, 0);
                    StartCoroutine(TrackEnvironmentReverse());
                }
            }
            else
            {
                environmentActive = false;
            }
        }
    }

    /// <summary>
    /// Updates the simple object's timescale to the default value after the single active time passes.
    /// Notifies the SimpleTargetAbilityTracker to forgot the saved frozen object.
    /// </summary>
    /// <param name="simpleObject"> object with a simple freeze mechanism </param>
    private IEnumerator CountdownSingleReverse(SimpleTimeManipulation simpleObject)
    {
        RemainingSingleActiveTime = freezeSingleTime;
        while(RemainingSingleActiveTime > 0)
        {
            RemainingSingleActiveTime -= Time.deltaTime;
            yield return null;
        }

        if (simpleObject != null)
        {
            
            simpleObject.UpdateTimeScale(1f);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private IEnumerator TrackEnvironmentReverse()
    {
        environmentActive = true;

        while (environmentActive && TimeStamina.singleton.Stamina > 0)
        {
            yield return waitForFixedUpdate;
        }

        environmentActive = false;
        MasterTime.singleton.UpdateTime(1);
    }
}
