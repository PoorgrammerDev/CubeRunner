using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TimeDilation : AbstractPowerUp {
    [Header("References")]
    [SerializeField] private PlayerPowerUp powerUpManager;
    [SerializeField] private Volume volume;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource droning;
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private PauseManager pauseManager;
    [SerializeField] private TimeDilationUPGData upgradeData;
    [SerializeField] private SaveManager saveManager;

    [Header("Stats")]
    [SerializeField] private float duration;
    [SerializeField] private float timeScale;

    private ChromaticAberration chromAb;
    private ColorAdjustments colorAdj;

    // Start is called before the first frame update
    void Start() {
        SetupUpgradeData();
        volume.sharedProfile.TryGet<ChromaticAberration>(out chromAb);
        volume.sharedProfile.TryGet<ColorAdjustments>(out colorAdj);
        ResetTDEffects(false);
    }

    void SetupUpgradeData() {
        int activePath = saveManager.GetActivePath(PowerUpType.TimeDilation);
        int upgLevel = saveManager.GetUpgradeLevel(PowerUpType.TimeDilation, activePath);
    
        TimeDilationUPGEntry upgradeEntry;
        switch (activePath) {
            case 0:
                upgradeEntry = upgradeData.leftPath[upgLevel];
                break;
            case 1:
                upgradeEntry = upgradeData.rightPath[upgLevel];
                break;
            default:
                Debug.LogError("Active Path for Upgrade TIMEDILATION returned incorrect value: " + activePath);
                return;
        }

        this.duration = upgradeEntry.duration;
        this.timeScale = 1.0f / upgradeEntry.slowRate;
    }

    public IEnumerator RunTimeDilation() {
        float t = 0f;
        powerUpManager.State = PowerUpState.Active;

        //pause music
        musicManager.Pause();

        //droning sound
        droning.clip = Sounds[1];
        droning.time = SoundStartTimes[1];
        droning.Play();

        //slowing down
        Time.timeScale = this.timeScale;

        //postprocessing effects
        chromAb.active = true;
        colorAdj.active = true;

        Color finalColor = new Color(0.9f, 0.9f, 1f);
        while (t <= 1) {
            t += 8 * Time.deltaTime;
            chromAb.intensity.SetValue(new NoInterpClampedFloatParameter(Mathf.Lerp(0, 1, t), 0, 1, true));
            colorAdj.saturation.SetValue(new ClampedFloatParameter(Mathf.Lerp(0, -45, t), -100, 100, true));
            colorAdj.colorFilter.Override(Color.Lerp(Color.white, finalColor, t));
            yield return null;
        }
        t = 0;

        //wait
        powerUpManager.ticker = duration;
        while (powerUpManager.ticker > 0) {
            powerUpManager.TopBar.value = (powerUpManager.ticker / duration);
            yield return null;
        }

        //speed back up sound
        audioSource.clip = Sounds[2];
        audioSource.Play();

        //speed back up and get rid of effects
        while (t <= 1) {
            if (!pauseManager.paused) {
                t += 2 * Time.deltaTime;
                chromAb.intensity.SetValue(new NoInterpClampedFloatParameter(Mathf.Lerp(1, 0, t), 0, 1, true));
                colorAdj.saturation.SetValue(new ClampedFloatParameter(Mathf.Lerp(-45, 0, t), -100, 100, true));
                colorAdj.colorFilter.Override(Color.Lerp(finalColor, Color.white, t));
                Time.timeScale = Mathf.Lerp(this.timeScale, 1, t);
            }
            yield return null;
        }

        //music returns
        droning.Stop();
        musicManager.Resume();
        StartCoroutine(musicManager.FadeIn(1));

        chromAb.active = false;
        colorAdj.active = false;
        powerUpManager.RemovePowerUp();
    }

    public void ResetTDEffects(bool stopCoroutine) {
        if (stopCoroutine) StopCoroutine(RunTimeDilation());

        chromAb.active = false;
        colorAdj.active = false;

        //hard clamp to default values
        Time.timeScale = 1f;
        chromAb.intensity.SetValue(new NoInterpClampedFloatParameter(0, 0, 1, true));
        colorAdj.saturation.SetValue(new ClampedFloatParameter(0, -100, 100, true));
    }
}
