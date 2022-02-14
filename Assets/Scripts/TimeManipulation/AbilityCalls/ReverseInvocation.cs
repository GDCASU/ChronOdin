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

    [Header("Quantities")]
    [SerializeField, Tooltip("The duration the object shall be reversed for")]
    private float _singleReverseTime = 3f;
    public float ReverseSingleTime { get => _singleReverseTime; }

    [SerializeField, Tooltip("Chunk of stamina consumed upon reversing a single object")]
    private float singleReverseStaminaCost = 1f;

    public float SingleReverseStaminaCost { get => singleReverseStaminaCost; }

    [SerializeField, Tooltip("Chunk of stamina consumed upon reversing the Player")]
    private float playerReverseStaminaCost = 1.5f;

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
    public void SimpleObjectReverse(SimpleTimeManipulation simpleObject)
    {
        if (simpleObject.SingleTimeScale < 0f)
            return;

        if (TimeStamina.singleton.ConsumeChunk(singleReverseStaminaCost))
            simpleObject.ActivateSingleObjectEffect(_singleReverseTime, TimeEffect.Reverse);
    }
    public void ComplexObjectReverse(ComplexTimeHub complexObject)
    {
        ComplexReverse complexReverse = complexObject.transform.GetComponent<ComplexReverse>();

        if (complexReverse == null || !complexReverse.ShouldReverse())
            return;


        if (TimeStamina.singleton.ConsumeChunk(singleReverseStaminaCost))
            complexObject.AffectObject(TimeEffect.Reverse, _singleReverseTime, -1f, true);
    }
    public void PlayerReverse()
    {
        if (playerReversal.PreviousPositionsCount < playerReversal.previousPositionsLimit)
            return;

        if (TimeStamina.singleton.ConsumeChunk(playerReverseStaminaCost))
            playerReversal.CallReverse();
    }
}
