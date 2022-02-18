/*
 * Contains an abstract class for reversing a complex gameobject.
 * 
 * Author: Cristion Dominguez
 * Date: 21 November 2021.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ComplexReverse : MonoBehaviour
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
    /// Returns whether the object should reverse.
    /// This method was implemented to prevent chain-reversing.
    /// </summary>
    public virtual bool ShouldReverse() => true;

    /// <summary>
    /// Reverses the gameobject for a specified time.
    /// </summary>
    /// <param name="reverseTime"> how long to reverse object </param>
    public abstract void Reverse(float reverseTime);

    /// <summary>
    /// Returns data other time effects may require.
    /// </summary>
    /// <returns> array of data points </returns>
    public virtual float[] GetData()
    {
        return null;
    }
}
