﻿/*
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

        private float originalVolume;
        private float currentVolume;

        void Start()
        {
            music = RuntimeManager.CreateInstance(Event);

            music.getVolume(out originalVolume);
            music.setVolume(1);

            music.start();
            music.release();

            RuntimeManager.PlayOneShot("event:/SFX/Fart");

            
        }
        void OnDestroy()
        {
            music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}