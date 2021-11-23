/*
 * Contains an abstract class for slowing a complex gameobject.
 * 
 * Author: Cristion Dominguez
 * Date: 21 November 2021.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ComplexSlow : MonoBehaviour
{
    /// <summary>
    /// The script to communicate between other time-related effects.
    /// </summary>
    protected ComplexTimeHub effectHub;

    /// <summary>
    /// Collects the hub from attached object.
    /// </summary>
    protected virtual void Awake() => effectHub = transform.GetComponent<ComplexTimeHub>();

    /// <summary>
    /// Slows the gameobject for a specified time.
    /// </summary>
    /// <param name="slowTime"> how long to slow the object </param>
    /// <param name="slowFactor"> how slow the object should be (i.e. 0.5 = half the speed) </param>
    public abstract void Slow(float slowTime, float slowFactor);

    /// <summary>
    /// Returns data other time effects may require.
    /// </summary>
    /// <returns> array of data points </returns>
    public virtual float[] GetData()
    {
        return null;
    }
}
