using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guidelines : AbstractPowerUp
{
    [Header("References")]
    [SerializeField] private PlayerPowerUp powerUpManager;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject guidelinesObject;
    [SerializeField] private GuidelinesUPGData upgradeData;
    [SerializeField] private SaveManager saveManager;

    [Header("Settings")]
    [SerializeField] private float duration;
    [SerializeField] private float length;
    [SerializeField] private LayerMask layerMask;
    float originalVolume;

    void Start () {
        SetupUpgradeData();
        originalVolume = PlayerPrefs.GetFloat(TagHolder.PREF_SFX_VOLUME);
    }

    void SetupUpgradeData() {
        int activePath = saveManager.GetActivePath(PowerUpType.Guidelines);
        int upgLevel = saveManager.GetUpgradeLevel(PowerUpType.Guidelines, activePath);
    
        GuidelinesUPGEntry upgradeEntry;
        switch (activePath) {
            case 0:
                upgradeEntry = upgradeData.leftPath[upgLevel];
                break;
            case 1:
                upgradeEntry = upgradeData.rightPath[upgLevel];
                break;
            default:
                Debug.LogError("Active Path for Upgrade GUIDELINES returned incorrect value: " + activePath);
                return;
        }

        this.duration = upgradeEntry.duration;
        this.length = upgradeEntry.range;
    }

    public IEnumerator RunGuidelines() {
        Transform objTrans = guidelinesObject.transform;
        Vector3 objScale = objTrans.localScale;
        Vector3 objPos = objTrans.localPosition;
        float t = 0f;

        //active state
        powerUpManager.State = PowerUpState.Active;
        
        StartCoroutine(SFXControl());
        
        //extend outwards
        guidelinesObject.SetActive(true);
        while (t <= 1) {
            t += 2f * Time.deltaTime;
            objScale.x = Mathf.Lerp(0, length, t);
            objPos.x = (objScale.x / 2f) + 0.4f;

            objTrans.localScale = objScale;
            objTrans.localPosition = objPos;
            yield return null;
        }

        //wait
        powerUpManager.ticker = duration;
        while (powerUpManager.ticker > 0) {
            powerUpManager.TopBar.value = (powerUpManager.ticker / duration);
            yield return null;
        }

        //retract
        t = 1f;
        while (t >= 0) {
            t -= 4f * Time.deltaTime;
            objScale.x = Mathf.Lerp(0, length, t);
            objPos.x = (objScale.x / 2f) + 0.4f;

            objTrans.localScale = objScale;
            objTrans.localPosition = objPos;
            yield return null;
        }
        
        guidelinesObject.SetActive(false);
        powerUpManager.RemovePowerUp();
    }

    IEnumerator SFXControl() {
        this.originalVolume = audioSource.volume;
        audioSource.clip = Sounds[0];
        audioSource.time = SoundStartTimes[0];
        audioSource.volume = 0;
        audioSource.loop = true;
        audioSource.Play();

        //fade in
        float t = 0f;
        while (t <= 1) {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, originalVolume, t);
            audioSource.pitch = Mathf.Lerp(0, 1, t);
            yield return null;
        }

        while (powerUpManager.GetActivePowerUp() == PowerUpType.Guidelines) {
            Collider[] obstacles = Physics.OverlapBox(guidelinesObject.transform.position, guidelinesObject.transform.localScale / 2f, guidelinesObject.transform.rotation, layerMask);
            float minDist = float.MaxValue;
            foreach (Collider obstacle in obstacles) {
                if (obstacle != null) {
                    float dist = (Mathf.Abs(obstacle.transform.position.x - powerUpManager.transform.position.x));
                    if (dist < minDist) {
                        minDist = dist;
                    }
                }
            }

            if (minDist == float.MaxValue) minDist = 0;
            else minDist = (length - minDist) / length;

            audioSource.pitch = Mathf.Lerp(1f, 1.25f, minDist);
            yield return null;
        }
    }

    public IEnumerator StopSound(bool instant) {
        if (!instant) {
            //fade out
            float t = 1f;
            while (t >= 0) {
                t -= Time.deltaTime;
                audioSource.volume = Mathf.Lerp(0, originalVolume, t);
                audioSource.pitch = Mathf.Lerp(0, 1, t);
                yield return null;
            }
        }

        audioSource.Stop();
        audioSource.clip = null;
        audioSource.loop = false;
        audioSource.pitch = 1f;
        audioSource.volume = this.originalVolume;
    }
}
