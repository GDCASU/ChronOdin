/*
 * Revision Author: Cristion Dominguez
 * Revision Date: 19 March 2021
 * 
 * Modification: Added a mute option that is enabled and disabled by the M key.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FMODUnity
{
    public class FMODStartMusic : MonoBehaviour
    {
        public EventReference Event;
        public static FMOD.Studio.EventInstance music;

        void Start()
        {
            music = RuntimeManager.CreateInstance(Event);

            music.setVolume(AudioVolumeValues.singleton.MusicVolume);

            music.start();
            //music.release();


            if (MainMenu.singleton)
            {
                MainMenu.singleton.musicUpdated += UpdateVolume;
            } 
            else
            {
                PauseMenu.singleton.musicUpdated += UpdateVolume;
            } 
        }
        void UpdateVolume()=>music.setVolume(AudioVolumeValues.singleton.MusicVolume);
        
        void OnDestroy()=>music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

}