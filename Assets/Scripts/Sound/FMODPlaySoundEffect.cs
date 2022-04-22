using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODPlaySoundEffect : MonoBehaviour
{
    public EventReference Event;
    public static FMOD.Studio.EventInstance effect;
    public float startingVolume;

    private float originalVolume;
    private float currentVolume;

    void Start()
    {
        effect = RuntimeManager.CreateInstance(Event);

        effect.setVolume(startingVolume);
        effect.getVolume(out originalVolume);
    }
    public void PlaySoundEffect()
    {
        effect.start();
        effect.release();
    }
    void OnDestroy()
    {
        effect.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
