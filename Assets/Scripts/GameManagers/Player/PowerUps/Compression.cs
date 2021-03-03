using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compression : AbstractPowerUp {
    [SerializeField] private PlayerPowerUp powerUpManager;
    [SerializeField] private Transform playerObject;
    [SerializeField] private float compressionDuration;
    [SerializeField] private float compressionSize;
    [SerializeField] private AudioSource audioSource;

    public IEnumerator RunCompression() {
        powerUpManager.State = PowerUpState.Active;
        Vector3 scale = playerObject.localScale;
        Vector3 position = playerObject.position;

        //save original scale
        float originalScale = scale.z;
        float originalY = position.y;
        float newY = (compressionSize / 2f) + 0.1f;

        //shrink scale to shrunk size
        float t = 0f;
        while (t <= 1) {
            t += 4 * Time.deltaTime;
            scale.x = scale.y = scale.z = Mathf.Lerp(originalScale, compressionSize, t);
            playerObject.localScale = scale;

            position = playerObject.position;
            position.y = Mathf.Lerp(originalY, newY, t);
            playerObject.position = position;
            yield return null;
        }

        //wait
        powerUpManager.ticker = compressionDuration;
        while (powerUpManager.ticker > 0) {
            powerUpManager.TopBar.value = (powerUpManager.ticker / compressionDuration);
            yield return null;
        }
        

        audioSource.clip = Sounds[1];
        audioSource.time = SoundStartTimes[1];
        audioSource.Play();

        //return to original size
        while (t >= 0) {
            t -= 2 * Time.deltaTime;
            scale.x = scale.y = scale.z = Mathf.Lerp(originalScale, compressionSize, t);
            playerObject.localScale = scale;

            position = playerObject.position;
            position.y = Mathf.Lerp(originalY, newY, t);
            playerObject.position = position;
            yield return null;
        }
        powerUpManager.RemovePowerUp();
    }
}
