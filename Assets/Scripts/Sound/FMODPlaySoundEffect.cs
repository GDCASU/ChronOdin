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
