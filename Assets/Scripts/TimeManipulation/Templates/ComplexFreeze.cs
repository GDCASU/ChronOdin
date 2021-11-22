/*
 * Contains an abstract class for freezing a complex gameobject.
 * 
 * Author: Cristion Dominguez
 * Date: 21 November 2021.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ComplexFreeze : MonoBehaviour
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
    /// Freezes the gameobject for a specified time.
    /// </summary>
    /// <param name="freezeTime"> how long to freeze object </param>
    public abstract void Freeze(float freezeTime);

    /// <summary>
    /// Returns data other time effects may require.
    /// </summary>
    /// <returns> array of data points </returns>
    public virtual float[] GetData()
    {
        return null;
    }
}
