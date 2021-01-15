using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TimeDilation : AbstractPowerUp {
    [SerializeField] private Volume volume;
    [SerializeField] private float timeDilationDuration;
    [SerializeField] private float timeDilationScale;
    ChromaticAberration chromAb;
    ColorAdjustments colorAdj;
    private PlayerPowerUp powerUpManager;

    // Start is called before the first frame update
    void Start() {
        powerUpManager = GetComponent<PlayerPowerUp>();
        volume.sharedProfile.TryGet<ChromaticAberration>(out chromAb);
        volume.sharedProfile.TryGet<ColorAdjustments>(out colorAdj);
        ResetTDEffects(false);
    }

    public IEnumerator RunTimeDilation() {
        float t = 0f;
        powerUpManager.State = PowerUpState.Active;


        //slowing down
        Time.timeScale = timeDilationScale;

        //postprocessing effects
        chromAb.active = true;
        colorAdj.active = true;
        while (t <= 1) {
            t += 8 * Time.deltaTime;
            chromAb.intensity.SetValue(new NoInterpClampedFloatParameter(Mathf.Lerp(0, 1, t), 0, 1, true));
            colorAdj.saturation.SetValue(new ClampedFloatParameter(Mathf.Lerp(0, -45, t), -100, 100, true));
            yield return null;
        }
        t = 0;

        //wait
        for (float i = 0; i < timeDilationDuration; i+= powerUpManager.TickDuration) {
            powerUpManager.TopBar.value = 1 - (i / timeDilationDuration);
            yield return powerUpManager.TickRT;
        }

        //speed back up and get rid of effects
        while (t <= 1) {
            t += 2 * Time.deltaTime;
            chromAb.intensity.SetValue(new NoInterpClampedFloatParameter(Mathf.Lerp(1, 0, t), 0, 1, true));
            colorAdj.saturation.SetValue(new ClampedFloatParameter(Mathf.Lerp(-45, 0, t), -100, 100, true));
            Time.timeScale = Mathf.Lerp(timeDilationScale, 1, t);
            yield return null;
        }

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
