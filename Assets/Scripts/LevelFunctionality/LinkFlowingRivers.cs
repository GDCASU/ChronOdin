using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkFlowingRivers : MonoBehaviour
{
    public List<Flowing_River> linkedRivers;

    public void ActivateAllLinkedRivers(float activeTime, TimeEffect effect)
    {
        foreach (Flowing_River river in linkedRivers) river.ActivateLinkedRiverEffect(activeTime, effect);
    }
}
