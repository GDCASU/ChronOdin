using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterTime : MonoBehaviour
{
    public static MasterTime singleton;
    public float timescale = 1;

    public delegate void UpdateTimeScale(float newTimescale);
    public event UpdateTimeScale updateTimeScaleEvent;
    private void Awake()
    {
        if (singleton == null)
            singleton = this;
        else
            Destroy(gameObject);
    }
    /// <summary>
    /// Changes the value of the timeScale variable and then calls the event so that other scripts can be aware of the change in timeSacle.
    /// Table: 
    /// value -1 should be passed for reverse Time: 
    /// value 0 should be passed for stopping Time:
    /// value 1 should be passed for reseting Time: 
    /// value 2 should be passed for accelerating Time: 
    /// value 5 should be passed For slowing down Time: 
    /// </summary>
    /// <returns></returns>
    public void UpdateTime(int value)
    {
        switch(value)
        {
            case -1:
                timescale = -1f;    //reverse
                break;
            case 0:
                timescale = 0f;     //stop
                break;
            case 1:
                timescale = 1f;     //reset
                break;
            case 2:
                timescale = 2f;     //accelerate
                break;
            case 5:
                timescale = .5f;    //slow down
                break;
        }
        if (updateTimeScaleEvent != null) updateTimeScaleEvent(timescale);
    }
}
