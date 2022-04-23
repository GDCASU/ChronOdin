using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODFootsetpSFXHandler : MonoBehaviour
{
    public static FMODFootsetpSFXHandler singleton;
    public EventReference WalkingEvent;
    public EventReference RunningEvent;
    public EventReference WaterEvent;
    private static FMOD.Studio.EventInstance walkEffect;
    private static FMOD.Studio.EventInstance runEffect;
    private static FMOD.Studio.EventInstance waterEffect;
    public float startingVolume;

    private float originalVolume;
    private float currentVolume;

    public int current_sfx;

    private void Awake()
    {
        if (singleton == null)
            singleton = this;
        else
            Destroy(gameObject);
    }
    public void StartWalkingSFX(int sfx)
    {
        switch (sfx)
        {
            case 1:
                walkEffect = RuntimeManager.CreateInstance(WalkingEvent);
                walkEffect.setVolume(startingVolume);
                walkEffect.getVolume(out originalVolume);

                walkEffect.start();
                walkEffect.release();
                current_sfx = 1;
                break;
            case 2:
                print("called");
                runEffect = RuntimeManager.CreateInstance(RunningEvent);
                runEffect.setVolume(startingVolume);
                runEffect.getVolume(out originalVolume);

                runEffect.start();
                runEffect.release();
                current_sfx = 2;
                break;
            case 3:
                waterEffect = RuntimeManager.CreateInstance(WaterEvent);
                waterEffect.setVolume(startingVolume);
                waterEffect.getVolume(out originalVolume);

                waterEffect.start();
                waterEffect.release();
                current_sfx = 3;
                break;

        }
    }
    public void StopSFX()
    {
        switch (current_sfx)
        {
            case 1:
                walkEffect.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                break;
            case 2:
                runEffect.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                break;
            case 3:
                waterEffect.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                break;

        }
        current_sfx = 0;
    }
    public void TransitionSFX(int next)
    {
        StopSFX();
        StartWalkingSFX(next);
    }
    void OnDestroy()
    {
        StopSFX();
    }
}
