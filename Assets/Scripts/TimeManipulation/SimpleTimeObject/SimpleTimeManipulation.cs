/*
 * Contains an abstract class for altering the time of a simple gameobject.
 * A simple gameobject is frozen, reversed, and slowed by the timescale value.
 * A timescale:
 *  equal to 1 causes the object to move normally.
 *  less than 0 causes the object to reverse.
 *  less than 1, but more than 0 causes the object to slow down.
 *  more than 1 causes the object to speed up.
 *  equal to 0 causes the object to freeze.
 *  
 *  Author: Cristion Dominguez
 *  Date: 21 Nov. 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SimpleTimeManipulation : MonoBehaviour
{
    /// <summary>
    /// Value that determines the gameobject's passage through time. timescale = 1 denotes normal,timescale < 0 denotes reverse, 0 < timescale < 1 denotes slow,
    /// timescale > 1 denotes speed, timescale = 0 denotes freeze.
    /// </summary>
    protected float timeScale = 1f;
    protected float singleTimeScale = 1;

    /// <summary>
    /// Subscribes the update timescale function to the MasterTime event for environment ability calls when the gameobject is enabled.
    /// </summary>
    protected virtual void OnEnable() => MasterTime.singleton.updateTimeScaleEvent += UpdateWithGlobalTimescale;

    /// <summary>
    /// Unsubscribes the update timescale function to the MasterTime event for environment ability calls when the gameobject is disabled.
    /// </summary>
    protected virtual void OnDisable() => MasterTime.singleton.updateTimeScaleEvent -= UpdateWithGlobalTimescale;

    /// <summary>
    /// Modifes the gameobject's timescale.
    /// </summary>
    /// <param name="globalScale"> the new timescale value to adopt </param>
    public virtual void UpdateWithGlobalTimescale(float globalScale)
    {
        if (singleTimeScale != 1) timeScale = singleTimeScale * globalScale;
        else timeScale = globalScale;

    }
    public virtual void ActivateSingleObjectEffect(float activeTime, TimeEffect effect) => StartCoroutine(SingleObjectEffect(activeTime, effect)); 
    private IEnumerator SingleObjectEffect(float activeTime, TimeEffect effect)
    {
        float timer = activeTime;
        float startingEffect = (int)effect / 10f;
        singleTimeScale = startingEffect;
        timeScale = singleTimeScale;
        while (timer > 0)
        {
            if (singleTimeScale != startingEffect) yield break;
            timer -= Time.deltaTime * MasterTime.singleton.timeScale;
            yield return null;
        }
        singleTimeScale = 1;
        timeScale = MasterTime.singleton.timeScale;
    }

}
