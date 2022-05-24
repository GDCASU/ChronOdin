using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODPlay3DEnviromentalSound : MonoBehaviour
{
    public EventReference Event;
    public static FMOD.Studio.EventInstance effect;
    public bool playOnStart;

    private void Start()
    {
        if(MainMenu.singleton)MainMenu.singleton.ambientUpdated += UpdateVolume;
        else PauseMenu.singleton.ambientUpdated += UpdateVolume;
        if (playOnStart) PlaySound();
    }
    public void PlaySound()
    {
        effect = RuntimeManager.CreateInstance(Event);

        effect.setVolume(AudioVolumeValues.singleton.AmbientVolume);
        effect.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        effect.start();
        effect.release();
    }
    void UpdateVolume()
    {
        effect.setVolume(AudioVolumeValues.singleton.AmbientVolume);
    }

    void OnDestroy()
    {
        effect.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        if (MainMenu.singleton) MainMenu.singleton.ambientUpdated -= UpdateVolume;
        else PauseMenu.singleton.ambientUpdated -= UpdateVolume;

    } 
    
}
