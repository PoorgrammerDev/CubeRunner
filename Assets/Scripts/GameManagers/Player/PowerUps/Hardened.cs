using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hardened : AbstractPowerUp
{
    [SerializeField] private PlayerPowerUp powerUpManager;
    [SerializeField] private float hardenedDuration;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private BarMove barMove;

    public GameObject shieldObject;
    private bool suspended = false;

    public void PlaySound() {
        audioSource.clip = Sounds[0];
        audioSource.time = SoundStartTimes[0];
        audioSource.Play();
    }

    public IEnumerator HardenedExpiration() {
        //wait
        powerUpManager.ticker = hardenedDuration;
        while (powerUpManager.GetActivePowerUp() == PowerUpType.Hardened && powerUpManager.ticker > 0) {
            if (!suspended) powerUpManager.BottomBar.value = (powerUpManager.ticker / hardenedDuration);
            yield return null;
        }

        //expire hardened
        if (powerUpManager.GetActivePowerUp() == PowerUpType.Hardened) powerUpManager.RemovePowerUp();
    }

    public void ResetExpiry() {
        //suspends regular bar animation, 
        suspended = true;
        StartCoroutine(barMove.MoveBarAsync(powerUpManager.BottomBar, 1, 4));
        StartCoroutine(DelayResume(4));

        powerUpManager.ticker = hardenedDuration;
    }

    //TODO quite messy code, find a better way to do this later
    IEnumerator DelayResume(float speed) {
        float t = 0f;
        while (t <= 1f) {
            t += speed * Time.deltaTime;
            yield return null;
        }

        suspended = false;
    }

    //NOTE: The actual shield mechanism is handled in PlayerCollision.cs
}
