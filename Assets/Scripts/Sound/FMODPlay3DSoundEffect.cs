using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODPlay3DSoundEffect : MonoBehaviour
{
    public EventReference Event;
    public static FMOD.Studio.EventInstance effect;
    public bool playOnStart;


    private void Start()
    {
        if (playOnStart) PlaySoundEffect();

        if (MainMenu.singleton)
        {
            MainMenu.singleton.sfxUpdated += UpdateVolume;
        }
        else
        {
            PauseMenu.singleton.sfxUpdated += UpdateVolume;
        } 
    }
    public void PlaySoundEffect()
    {
        effect = RuntimeManager.CreateInstance(Event);

        effect.setVolume(AudioVolumeValues.singleton.SFXVolume);
        effect.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        effect.start();
        effect.release();
    }
    void UpdateVolume()=>effect.setVolume(AudioVolumeValues.singleton.SFXVolume);
    void OnDestroy()=>effect.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
}
