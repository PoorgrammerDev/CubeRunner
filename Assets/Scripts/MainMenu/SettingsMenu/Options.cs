using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    [SerializeField] private Slider SFXVolume;
    [SerializeField] private Slider MusicVolume;
    [SerializeField] private Toggle ColorblindMode;
    [SerializeField] private Slider Particles;
    [SerializeField] private GameValues gameValues;
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private SFXVolumeManager[] soundEffectManagers;

    void Start() {
        SFXVolume.value = PlayerPrefs.GetFloat(TagHolder.PREF_SFX_VOLUME, 0.5f);
        MusicVolume.value = PlayerPrefs.GetFloat(TagHolder.PREF_MUSIC_VOLUME, 0.5f);
        ColorblindMode.isOn = (PlayerPrefs.GetInt(TagHolder.PREF_COLORBLIND_MODE, 0) == 1);
        Particles.value = PlayerPrefs.GetInt(TagHolder.PREF_PARTICLES, 2);
    }

    public void UpdateSFXVolume() {
        PlayerPrefs.SetFloat(TagHolder.PREF_SFX_VOLUME, SFXVolume.value);
        
        foreach (SFXVolumeManager soundEffectManager in soundEffectManagers) {
            soundEffectManager.UpdateVolume();
        }
    }

    public void UpdateMusicVolume() {
        PlayerPrefs.SetFloat(TagHolder.PREF_MUSIC_VOLUME, MusicVolume.value);
        musicManager.UpdateVolume();
    }

    public void UpdateColorblindMode() {
        PlayerPrefs.SetInt(TagHolder.PREF_COLORBLIND_MODE, ColorblindMode.isOn ? 1 : 0);
    }

    public void UpdateParticles() {
        PlayerPrefs.SetInt(TagHolder.PREF_PARTICLES, (int) Particles.value);
        if (gameValues != null) {
            gameValues.Divide = (uint) Particles.value;
        }
    }
}
