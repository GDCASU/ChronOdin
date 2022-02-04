/*
 * A component (child) of the pendulum that updates the entire pendulum's time scale when the component is targeted by a Player's time ability (does not include time environment events).
 * 
 * Author: Cristion Dominguez
 * Date: 21 Dec. 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendulumComponent : SimpleTimeManipulation
{
    private Pendulum pendulum;

    /// <summary>
    /// Gathers the pendulum.
    /// </summary>
    private void Awake()
    {
        pendulum = transform.GetComponentInParent<Pendulum>();
    }

    /// <summary>
    /// Updates the pendulum's time scale when the attached pendulum component is targeted by a Player's time ability.
    /// </summary>
    /// <param name="newTimescale"></param>
    public override void UpdateTimeScale(float newTimescale)
    {
        pendulum.UpdateTimeScale(newTimescale);
    }

    /// <summary>
    /// Ensures the pendulum components are not subscribed to the time environment events.
    /// </summary>
    protected override void OnEnable() { }
    protected override void OnDisable() { }
}