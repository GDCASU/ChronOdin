using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface LinkedToPressButton
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
