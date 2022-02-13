/*
 * Reverses a single object or the Player for a specified amount of time.
 * To revse a single object, the Player must look at the object and press the single object reverse button, which instantly consumes a chunk of stamina.
 * To reverse the Player, the Player must press the player reverse button, which consumes all the available stamina.
 * 
 * Author: Cristion Dominguez
 * Date: 21 November 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ReverseInvocation : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField, Tooltip("The button to reverse a single object")]
    private KeyCode singleReverseButton = KeyCode.R;

    [SerializeField, Tooltip("The button to reverse the Player")]
    private KeyCode playerReverseButton = KeyCode.E;

    [Header("Quantities")]
    [SerializeField, Tooltip("The duration the object shall be reversed for")]
    private float _singleReverseTime = 3f;
    public float ReverseSingleTime { get => _singleReverseTime; }

    [SerializeField, Tooltip("Chunk of stamina consumed upon reversing a single object")]
    private float singleReverseStaminaCost = 1f;

    [SerializeField, Tooltip("Chunk of stamina consumed upon reversing the Player")]
    private float playerReverseStaminaCost = 1.5f;

    SimpleTimeManipulation simpleObject = null;  // object with a simple reverse mechanism
    ComplexTimeHub complexObject = null;  // object with a complex reverse mechanism
    PlayerReverse playerReversal = null;  // script attached to Player responsible for reversing the Player

    public static ReverseInvocation singleton;

    /// <summary>
    /// Sets up singleton and collects the PlayerReverse script.
    /// </summary>
    private void Start()
    {
        if (singleton == null)
            singleton = this;
        else
            Destroy(gameObject);

        playerReversal = transform.GetComponent<PlayerReverse>();
    }

    /// <summary>
    /// Reverses a single object or the Player depending on Player input and the ability to reverse.
    /// </summary>
    private void Update()
    {
        // If the Player presses the single reverse button, attempt to reverse a single object.
        if (Input.GetKeyDown(singleReverseButton))
        {
            // If the environment is undergoing a time effect, do not attempt to reverse a single object.
            if (MasterTime.singleton.timeScale != 1f)
                return;

            // Acquire the transform of the object observed by the Player. If a transform is not acquired, do nothing.
            Transform someObject = PlayerInteractions.singleton.RaycastTransform();
            if (someObject == null)
                return;

            // If the transform belongs to a simple time object, is not reversing, and the Player has stamina, reverse the object.
            simpleObject = someObject.transform.GetComponent<SimpleTimeManipulation>();
            if (simpleObject != null)
            {
                if (simpleObject.SingleTimeScale < 0f)
                    return;

                if (TimeStamina.singleton.ConsumeChunk(singleReverseStaminaCost))
                    simpleObject.ActivateSingleObjectEffect(_singleReverseTime, TimeEffect.Reverse);

                return;
            }

            // If the transform belongs to a complex time object that can be reversed, is not reversing, and the Player has stamina, reverse the object.
            complexObject = someObject.transform.GetComponent<ComplexTimeHub>();
            if (complexObject != null)
            {
                ComplexReverse complexReverse = complexObject.transform.GetComponent<ComplexReverse>();

                if (complexReverse == null || !complexReverse.ShouldReverse())
                    return;
                    

                if (TimeStamina.singleton.ConsumeChunk(singleReverseStaminaCost))
                    complexObject.AffectObject(TimeEffect.Reverse, _singleReverseTime, -1f, true);
            }
        }

        // If the Player presses the reverse Player button, has enough prevoious positions to reverse to, and has stamina, reverse the Player.
        else if (Input.GetKeyDown(playerReverseButton))
        {
            if (playerReversal.PreviousPositionsCount < playerReversal.previousPositionsLimit)
                return;

            if (TimeStamina.singleton.ConsumeChunk(playerReverseStaminaCost))
                playerReversal.CallReverse();
        }
    }
}
