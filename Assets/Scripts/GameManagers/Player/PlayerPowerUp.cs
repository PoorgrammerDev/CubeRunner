﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

enum PowerUpState {
    Empty,
    Standby,
    Active,
}

public class PlayerPowerUp : MonoBehaviour
{
    [SerializeField] private GameValues gameValues;
    [SerializeField] private Volume volume;
    [SerializeField] private PowerUpState state;
    [SerializeField] private PowerUpType type;
    [SerializeField] private GameObject blaster;
    [SerializeField] private GameObject hardened;

    [Header("Time Dilation")]
    [SerializeField] private float timeDilationDuration;
    [SerializeField] private float timeDilationScale;
    ChromaticAberration chromAb;
    ColorAdjustments colorAdj;


    void Start() {
        volume.sharedProfile.TryGet<ChromaticAberration>(out chromAb);
        volume.sharedProfile.TryGet<ColorAdjustments>(out colorAdj);
        ResetTDEffects(false);
    }
    
    public bool AddPowerUp (PowerUpType type) {
        if (state != PowerUpState.Active) {
            if (state == PowerUpState.Standby) {
                if (this.type == type) return false;
                RemovePowerUp();
            }

            this.type = type;
            state = PowerUpState.Standby;

            if (type == PowerUpType.Hardened) {
                hardened.SetActive(true);
            }
            return true;
        }
        return false;
    }

    public void RemovePowerUp() {
        if (state != PowerUpState.Empty) {
            state = PowerUpState.Empty;

            if (type == PowerUpType.Hardened) {
                hardened.SetActive(false);
            }
        }
    }

    public PowerUpType? GetActivePowerUp () {
        if (state != PowerUpState.Empty) {
            return type;
        }
        return null;
    }

    void Update() {
        if (!gameValues.GameActive) return;

        if (Input.GetKeyDown(KeyCode.F)) {
            if (state == PowerUpState.Standby) {
                PowerUpType type = this.type;
                if (type == PowerUpType.TimeDilation) {
                    StartCoroutine(RunTimeDilation());
                }
            }
        }
    }

    //TIME DILATION--------------------------------------
    IEnumerator RunTimeDilation() {
        float t = 0f;
        state = PowerUpState.Active;


        //slowing down
        Time.timeScale = timeDilationScale;

        //postprocessing effects
        chromAb.active = true;
        colorAdj.active = true;
        while (t < 1) {
            chromAb.intensity.SetValue(new NoInterpClampedFloatParameter(Mathf.Lerp(0, 1, t), 0, 1, true));
            colorAdj.saturation.SetValue(new ClampedFloatParameter(Mathf.Lerp(0, -45, t), -100, 100, true));
            t += 8 * Time.deltaTime;
            yield return null;
        }

        t = 0;

        //wait
        yield return new WaitForSeconds(timeDilationDuration);

        //speed back up and get rid of effects
        while (t < 1) {
            chromAb.intensity.SetValue(new NoInterpClampedFloatParameter(Mathf.Lerp(1, 0, t), 0, 1, true));
            colorAdj.saturation.SetValue(new ClampedFloatParameter(Mathf.Lerp(-45, 0, t), -100, 100, true));
            Time.timeScale = Mathf.Lerp(timeDilationScale, 1, t);
            t += 2 * Time.deltaTime;
            yield return null;
        }

        RemovePowerUp();
        ResetTDEffects(false);
    }

    public void ResetTDEffects(bool stopCoroutine) {
        if (stopCoroutine) StopCoroutine(RunTimeDilation());

        //hard clamp values back
        Time.timeScale = 1f;
        chromAb.intensity.SetValue(new NoInterpClampedFloatParameter(0, 0, 1, true));
        colorAdj.saturation.SetValue(new ClampedFloatParameter(0, -100, 100, true));
        chromAb.active = false;
        colorAdj.active = false;
    }

    //
}
