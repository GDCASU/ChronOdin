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
    [Header("Button")]
    [SerializeField, Tooltip("The button to freeze a single object")]
    private KeyCode singleSlowButton = KeyCode.B;

    [SerializeField, Tooltip("The button to slow the entire environment")]
    private KeyCode environmentSlowButton = KeyCode.V;

    [Header("Quantities")]
    [SerializeField, Tooltip("The time the object shall be slowed for")]
    private float _singleSlowTime = 3f;
    public float SingleSlowTime { get => _singleSlowTime; }

    [SerializeField, Tooltip("How slow the object shall be")]
    private float slowFactor = 0.5f;

    [SerializeField, Tooltip("Chunk of stamina consumed upon slowing a single object")]
    private float singleSlowStaminaCost = 1f;

    [SerializeField, Tooltip("RStamina drained per second after slowing the environment")]
    private float environmentSlowStaminaRate = 1f;

    SimpleTimeManipulation simpleObject = null;  // simple time object
    ComplexTimeHub complexObject = null;  // complex time object
    public static Action<TimeEffect, float, float, bool> slowAllComplexObjects;  // event container for slowing every complex time object

    private bool environmentSlowToggledOn = false;  // Has the Player toggled environment slow on?
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

    /// <summary>
    /// Slows a single object or the environment depending on Player input and the ability to slow a specific object(s).
    /// </summary>
    private void Update()
    {
        // If the Player presses the single slow button, attempt to slow a single object.
        if (Input.GetKeyDown(singleSlowButton))
        {
            // If the environment is undergoing a time effect, do not attempt to slow a single object.
            if (MasterTime.singleton.timeScale != 1f)
                return;

            // Acquire the transform of the object observed by the Player. If a transform is not acquired, do nothing.
            Transform someObject = PlayerInteractions.singleton.RaycastTransform();
            if (someObject == null)
                return;

            // If the transform belongs to a simple time object and the Player has stamina, slow the object.
            simpleObject = someObject.transform.GetComponent<SimpleTimeManipulation>();
            if (simpleObject != null)
            {
                if (TimeStamina.singleton.ConsumeChunk(singleSlowStaminaCost))
                    simpleObject.ActivateSingleObjectEffect(_singleSlowTime, TimeEffect.Slow);

                return;
            }

            // If the transform belongs to a complex time object that can be slowed and the Player has stamina, slow the object.
            complexObject = someObject.transform.GetComponent<ComplexTimeHub>();
            if (complexObject != null)
            {
                if (complexObject.transform.GetComponent<ComplexSlow>() == null)
                    return;

                if (TimeStamina.singleton.ConsumeChunk(singleSlowStaminaCost))
                    complexObject.AffectObject(TimeEffect.Slow, _singleSlowTime, slowFactor, true);
            }
        }

        // If the Player presses the environment slow button, attempt to slow or unslow the environment.
        else if (Input.GetKeyDown(environmentSlowButton))
        {
            // If the environment is undergoing a time effect that is not environment slow, do not attempt to slow the environment.
            if (MasterTime.singleton.timeScale > 1f || MasterTime.singleton.timeScale <= 0f)
                return;

            // If the environment slow ability is toggled off and the Player has stamina, freeze the environment and commence draining stamina.
            if (!environmentSlowToggledOn)
            {
                if (TimeStamina.singleton.CommenceDraining(environmentSlowStaminaRate))
                {
                    MasterTime.singleton.UpdateTime((int) TimeEffect.Slow);
                    slowAllComplexObjects?.Invoke(TimeEffect.Slow, TimeStamina.singleton.RemainingDrainTime, slowFactor, false);
                    environmentSlowToggledOn = true;
                    StartCoroutine(TrackEnvironmentSlow());
                }
            }
            // If the environment slow ability is toggled on, then unslow the environment.
            else
            {
                environmentSlowToggledOn = false;
            }
        }
    }

    /// <summary>
    /// Constantly checks that the conditions for environment slow are satisfied.
    /// If the Player toggled off environment slow or has no more stamina, then the environment is unslowed and stamina is no longer draining.
    /// </summary>
    private IEnumerator TrackEnvironmentSlow()
    {
        while (environmentSlowToggledOn && TimeStamina.singleton.Stamina > 0)
            yield return waitForFixedUpdate;

        MasterTime.singleton.UpdateTime((int)TimeEffect.None);
        slowAllComplexObjects?.Invoke(TimeEffect.None, 0f, 1f, false);

        // If the Player toggled off environment slow, then halt draining.
        // Otherwise, toggle off the environment slow automatically.
        if (!environmentSlowToggledOn)
        {
            TimeStamina.singleton.HaltDraining();
        }
        else
        {
            environmentSlowToggledOn = false;
        }
    }
}
