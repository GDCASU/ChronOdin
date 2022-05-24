using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODPlaySoundEffect : MonoBehaviour
{
    public EventReference Event;
    public static FMOD.Studio.EventInstance effect;

    void Start()
    {
        if (MainMenu.singleton)MainMenu.singleton.sfxUpdated += UpdateVolume;
        else PauseMenu.singleton.sfxUpdated += UpdateVolume;
    }
    public void PlaySoundEffect()
    {
        effect = RuntimeManager.CreateInstance(Event);

        effect.setVolume(AudioVolumeValues.singleton.SFXVolume);
        effect.start();
        effect.release();
    }
    void UpdateVolume()=>effect.setVolume(AudioVolumeValues.singleton.SFXVolume);
    
    void OnDestroy()=> effect.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
}
