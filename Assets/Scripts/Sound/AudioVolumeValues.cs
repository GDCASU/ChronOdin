using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVolumeValues : MonoBehaviour
{
    public static AudioVolumeValues singleton;
    [SerializeField] private float musicVolume = .7f;

    public float MusicVolume
    {
        get { return musicVolume; }
        set { musicVolume = value; }
    }
    [SerializeField]private float sfxVolume = .5f;
    public float SFXVolume
    {
        get { return sfxVolume; }
        set { sfxVolume = value; }
    }
    [SerializeField] private float ambientVolume = .3f;
    public float AmbientVolume
    {
        get { return ambientVolume; }
        set { ambientVolume = value; }
    }
    private void Awake()
    {
        if (singleton == null)
            singleton = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}
