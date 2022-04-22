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

        if (MainMenu.singleton)
        {
            originalVolume = MainMenu.singleton.sfx;
            MainMenu.singleton.sfxUpdated += UpdateVolume;
        }
        else
        {
            originalVolume = PauseMenu.singleton.sfx;
            PauseMenu.singleton.sfxUpdated += UpdateVolume;
        } 
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
    void UpdateVolume()
    {
        currentVolume = (MainMenu.singleton) ? MainMenu.singleton.sfx : PauseMenu.singleton.sfx;
        effect.setVolume(currentVolume);
    }
    void OnDestroy()
    {
        effect.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
