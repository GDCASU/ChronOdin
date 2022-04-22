using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODPlay3DSoundEffect : MonoBehaviour
{
    public EventReference Event;
    public static FMOD.Studio.EventInstance effect;
    public float startingVolume;
    public bool playOnStart;

    private float originalVolume;
    private float currentVolume;

    private void Start()
    {
        if (playOnStart) PlaySoundEffect();
    }
    public void PlaySoundEffect()
    {
        effect = RuntimeManager.CreateInstance(Event);

        effect.setVolume(startingVolume);
        effect.getVolume(out originalVolume);
        effect.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        effect.start();
        effect.release();
    }
    void OnDestroy()
    {
        effect.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
