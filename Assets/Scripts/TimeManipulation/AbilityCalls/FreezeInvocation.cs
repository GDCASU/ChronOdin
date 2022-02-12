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
    [Header("Buttons")]
    [SerializeField, Tooltip("The button to freeze a single object")]
    private KeyCode singleFreezeButton = KeyCode.F;

    [SerializeField, Tooltip("The button to freeze the entire environment")]
    private KeyCode environmentFreezeButton = KeyCode.G;

    [Header("Quantities")]
    [SerializeField, Tooltip("The time the object shall be frozen for")]
    private float _singleFreezeTime = 3f;
    public float SingleFreezeTime { get => _singleFreezeTime; }

    [SerializeField, Tooltip("Chunk of stamina consumed upon freezing a single object")]
    private float singleFreezeStaminaCost = 1f;

    [SerializeField, Tooltip("Stamina drained per second after freezing the environment")]
    private float environmentFreezeStaminaRate = 1f;

    SimpleTimeManipulation simpleObject = null;  // simple time object
    ComplexTimeHub complexObject = null;  // complex time object
    public static Action<TimeEffect, float, float, bool> freezeAllComplexObjects;  // event container for freezing every complex time object

    private bool environmentFreezeToggledOn = false;  // Has the Player toggled environment freeze on?
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

    /// <summary>
    /// Freezes a single object or the environment depending on Player input and the ability to freeze a specific object(s).
    /// </summary>
    private void Update()
    {
        // If the Player presses the single freeze button, attempt to freeze a single object.
        if (Input.GetKeyDown(singleFreezeButton))
        {
            // If the environment is undergoing a time effect, do not attempt to freeze a single object.
            if (MasterTime.singleton.timeScale != 1f)
                return;

            // Acquire the transform of the object observed by the Player. If a transform is not acquired, do nothing.
            Transform someObject = PlayerInteractions.singleton.RaycastTransform();
            if (someObject == null)
                return;


            // If the transform belongs to a simple time object and the Player has stamina, freeze the object.
            simpleObject = someObject.transform.GetComponent<SimpleTimeManipulation>();
            if (simpleObject != null)
            {
                if (TimeStamina.singleton.ConsumeChunk(singleFreezeStaminaCost))
                    simpleObject.ActivateSingleObjectEffect(_singleFreezeTime, TimeEffect.Freeze);

                return;
            }

            // If the transform belongs to a complex time object that can be frozen and the Player has stamina, freeze the object.
            complexObject = someObject.transform.GetComponent<ComplexTimeHub>();
            if (complexObject != null)
            {
                if (complexObject.transform.GetComponent<ComplexFreeze>() == null)
                    return;

                if (TimeStamina.singleton.ConsumeChunk(singleFreezeStaminaCost))
                    complexObject.AffectObject(TimeEffect.Freeze, _singleFreezeTime, 0f, true);
            }
        }

        // If the Player presses the environment freeze button, attempt to freeze or unfreeze the environment.
        else if (Input.GetKeyDown(environmentFreezeButton))
        {
            // If the environment is undergoing a time effect that is not environment freeze, do not attempt to freeze the environment.
            if (MasterTime.singleton.timeScale != 1f && MasterTime.singleton.timeScale != 0f)
                return;

            // If the environment freeze ability is toggled off and the Player has stamina, freeze the environment and commence draining stamina.
            if (!environmentFreezeToggledOn)
            {
                if (TimeStamina.singleton.CommenceDraining(environmentFreezeStaminaRate))
                {
                    MasterTime.singleton.UpdateTime((int) TimeEffect.Freeze);
                    freezeAllComplexObjects?.Invoke(TimeEffect.Freeze, TimeStamina.singleton.RemainingDrainTime, 0, false);
                    environmentFreezeToggledOn = true;
                    StartCoroutine(TrackEnvironmentFreeze());
                }
            }
            // If the environment freeze ability is toggled on, then unfreeze the environment.
            else
            {
                environmentFreezeToggledOn = false;
            }
        }
    }

    /// <summary>
    /// Constantly checks that the conditions for environment freeze are satisfied.
    /// If the Player toggled off environment freeze or has no more stamina, then the environment is unfrozen and stamina is no longer draining.
    /// </summary>
    private IEnumerator TrackEnvironmentFreeze()
    {
        while (environmentFreezeToggledOn && TimeStamina.singleton.Stamina > 0f)
            yield return waitForFixedUpdate;

        MasterTime.singleton.UpdateTime((int)TimeEffect.None);
        freezeAllComplexObjects?.Invoke(TimeEffect.None, 0f, 1f, false);

        // If the Player toggled off environment freeze, then halt draining.
        // Otherwise, toggle off the environment freeze automatically.
        if (!environmentFreezeToggledOn)
        {
            TimeStamina.singleton.HaltDraining();
        }
        else
        {
            environmentFreezeToggledOn = false;
        }
    }
}
