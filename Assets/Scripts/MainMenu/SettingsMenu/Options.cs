using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    [SerializeField] private GameValues gameValues;
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private SFXVolumeManager[] soundEffectManagers;

    [Header("UI Elements")]
    [SerializeField] private Slider SFXVolume;
    [SerializeField] private Slider MusicVolume;
    [SerializeField] private Toggle ColorblindMode;
    [SerializeField] private Slider Graphics;
    [SerializeField] private Toggle SkipAnimations;

    void Start() {
        SFXVolume.value = PlayerPrefs.GetFloat(TagHolder.PREF_SFX_VOLUME, 0.5f);
        MusicVolume.value = PlayerPrefs.GetFloat(TagHolder.PREF_MUSIC_VOLUME, 0.5f);
        ColorblindMode.isOn = (PlayerPrefs.GetInt(TagHolder.PREF_COLORBLIND_MODE, 0) == 1);
        Graphics.value = PlayerPrefs.GetInt(TagHolder.PREF_GRAPHICS, 1);
        SkipAnimations.isOn = (PlayerPrefs.GetInt(TagHolder.PREF_SKIP_ANIM, 0) == 1);
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
