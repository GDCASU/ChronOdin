using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An interface for objects that are affected by a pressure plate.
/// 
/// Author: Alben Trang
/// </summary>
public interface PressureObject
{
    /// <summary>
    /// Change the object to an activated(alternative) state.
    /// </summary>
    public void Activate();

    /// <summary>
    /// Have the object return to it's original state.
    /// </summary>
    public void Deactivate();
}
