using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compression : AbstractPowerUp {
    [SerializeField] private float compressionDuration;
    [SerializeField] private float compressionSize;
    [SerializeField] private AudioSource audioSource;
    private PlayerPowerUp powerUpManager;

    void Start() {
        powerUpManager = GetComponent<PlayerPowerUp>();
    }

    public IEnumerator RunCompression() {
        powerUpManager.State = PowerUpState.Active;
        Vector3 scale = transform.localScale;
        Vector3 position = transform.position;

        //save original scale
        float originalScale = scale.z;
        float originalY = position.y;
        float newY = (compressionSize / 2f) + 0.1f;

        //shrink scale to shrunk size
        float t = 0f;
        while (t <= 1) {
            t += 4 * Time.deltaTime;
            scale.x = scale.y = scale.z = Mathf.Lerp(originalScale, compressionSize, t);
            transform.localScale = scale;

            position = transform.position;
            position.y = Mathf.Lerp(originalY, newY, t);
            transform.position = position;
            yield return null;
        }

        //wait
        for (float i = 0; i < compressionDuration; i+= powerUpManager.TickDuration) {
            powerUpManager.TopBar.value = 1 - (i / compressionDuration);
            yield return powerUpManager.Tick;
        }
        

        audioSource.clip = Sounds[1];
        audioSource.time = SoundStartTimes[1];
        audioSource.Play();

        //return to original size
        while (t >= 0) {
            t -= 2 * Time.deltaTime;
            scale.x = scale.y = scale.z = Mathf.Lerp(originalScale, compressionSize, t);
            transform.localScale = scale;

            position = transform.position;
            position.y = Mathf.Lerp(originalY, newY, t);
            transform.position = position;
            yield return null;
        }
        powerUpManager.RemovePowerUp();
    }
}
