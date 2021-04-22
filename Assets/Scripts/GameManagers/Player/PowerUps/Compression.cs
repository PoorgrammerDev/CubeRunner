using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compression : AbstractPowerUp {
    [Header("References")]
    [SerializeField] private PlayerPowerUp powerUpManager;
    [SerializeField] private Transform playerObject;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private CompressionUPGData upgradeData;
    [SerializeField] private SaveManager saveManager;

    [Header("Stats")]
    [SerializeField] private float duration;
    [SerializeField] private float size;

    void Start() {
        int activePath = saveManager.GetActivePath(PowerUpType.Compression);
        int upgLevel = saveManager.GetUpgradeLevel(PowerUpType.Compression, activePath);

        CompressionUPGEntry upgradeEntry;
        switch (activePath) {
            case 0:
                upgradeEntry = upgradeData.leftPath[upgLevel];
                break;
            case 1:
                upgradeEntry = upgradeData.rightPath[upgLevel];
                break;
            default:
                Debug.LogError("Active Path for Upgrade COMPRESSION returned incorrect value: " + activePath);
                return;
        }

        this.duration = upgradeEntry.duration;
        this.size = 1.0f / upgradeEntry.shrinkRate;
    }

    public IEnumerator RunCompression() {
        powerUpManager.State = PowerUpState.Active;
        Vector3 scale = playerObject.localScale;
        Vector3 position = playerObject.position;

        //save original scale
        float originalScale = scale.z;
        float originalY = position.y;
        float newY = (this.size / 2f) + 0.1f;

        //shrink scale to shrunk size
        float t = 0f;
        while (t <= 1) {
            t += 4 * Time.deltaTime;
            scale.x = scale.y = scale.z = Mathf.Lerp(originalScale, this.size, t);
            playerObject.localScale = scale;

            position = playerObject.position;
            position.y = Mathf.Lerp(originalY, newY, t);
            playerObject.position = position;
            yield return null;
        }

        //wait
        powerUpManager.ticker = this.duration;
        while (powerUpManager.ticker > 0) {
            powerUpManager.TopBar.value = (powerUpManager.ticker / this.duration);
            yield return null;
        }
        

        audioSource.clip = Sounds[1];
        audioSource.time = SoundStartTimes[1];
        audioSource.Play();

        //return to original size
        while (t >= 0) {
            t -= 2 * Time.deltaTime;
            scale.x = scale.y = scale.z = Mathf.Lerp(originalScale, this.size, t);
            playerObject.localScale = scale;

            position = playerObject.position;
            position.y = Mathf.Lerp(originalY, newY, t);
            playerObject.position = position;
            yield return null;
        }
        powerUpManager.RemovePowerUp();
    }
}
