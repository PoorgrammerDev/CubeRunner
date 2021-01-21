using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//THIS SCRIPT IS TO BE PLACED DIRECTLY ON THE MUSIC AUDIO SOURCE GAMEOBJECT
public class SFXVolumeManager : MonoBehaviour
{
    private AudioSource audioSource;
    private const float defaultSFXVolume = 0.5f;
    // Start is called before the first frame update
    void Start() {
        audioSource = GetComponent<AudioSource>();
        UpdateVolume();
    }

    public void UpdateVolume() {
        audioSource.volume = PlayerPrefs.GetFloat(TagHolder.PREF_SFX_VOLUME, defaultSFXVolume);
    }
}
