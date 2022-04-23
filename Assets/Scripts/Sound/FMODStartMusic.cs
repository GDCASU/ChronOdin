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
        public float startingVolume;

        public float originalVolume;
        private float currentVolume;

        bool inMain;

        void Start()
        {
            music = RuntimeManager.CreateInstance(Event);

            music.setVolume(startingVolume);
            music.getVolume(out originalVolume);

            music.start();
            music.release();

            if (MainMenu.singleton)
            {
                originalVolume = MainMenu.singleton.music;
                MainMenu.singleton.musicUpdated += UpdateVolume;
            } 
            else
            {
                originalVolume = PauseMenu.singleton.music;
                PauseMenu.singleton.musicUpdated += UpdateVolume;
            } 
        }
        void UpdateVolume()
        {
            currentVolume = (MainMenu.singleton) ? MainMenu.singleton.music : PauseMenu.singleton.music;
            music.setVolume(currentVolume);
        }
        void OnDestroy()
        {
            music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

}