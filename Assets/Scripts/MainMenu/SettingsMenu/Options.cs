using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    [SerializeField] private GameValues gameValues;
    [SerializeField] private MusicManager musicManager;

    private SFXVolumeManager[] soundEffectManagers;

    [Header("UI Elements")]
    [SerializeField] private Slider SFXVolume;
    [SerializeField] private Slider MusicVolume;
    [SerializeField] private Slider Graphics;
    [SerializeField] private Toggle SkipAnimations;

    void Awake() {
        SFXVolume.value = PlayerPrefs.GetFloat(TagHolder.PREF_SFX_VOLUME, 0.5f);
        MusicVolume.value = PlayerPrefs.GetFloat(TagHolder.PREF_MUSIC_VOLUME, 0.5f);
        Graphics.value = PlayerPrefs.GetInt(TagHolder.PREF_GRAPHICS, 2);
        SkipAnimations.isOn = (PlayerPrefs.GetInt(TagHolder.PREF_SKIP_ANIM, 0) == 1);
    }

    public void UpdateSFXVolume() {
        PlayerPrefs.SetFloat(TagHolder.PREF_SFX_VOLUME, SFXVolume.value);
        
        if (soundEffectManagers == null) {
            soundEffectManagers = FindObjectsOfType<SFXVolumeManager>();
        }

        foreach (SFXVolumeManager manager in soundEffectManagers) {
            if (manager != null) manager.UpdateVolume();
        }
    }

    public void UpdateMusicVolume() {
        PlayerPrefs.SetFloat(TagHolder.PREF_MUSIC_VOLUME, MusicVolume.value);
        musicManager.UpdateVolume();
    }

    public void UpdateGraphics() {
        PlayerPrefs.SetInt(TagHolder.PREF_GRAPHICS, (int) Graphics.value);
        if (gameValues != null) {
            gameValues.Divide = (uint) Graphics.value;
        }
    }

    public void UpdateSkipAnimations() {
        PlayerPrefs.SetInt(TagHolder.PREF_SKIP_ANIM, SkipAnimations.isOn ? 1 : 0);
    }
}
