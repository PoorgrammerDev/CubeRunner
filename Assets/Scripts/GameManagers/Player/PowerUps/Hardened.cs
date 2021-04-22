using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hardened : AbstractPowerUp
{
    [Header("References")]
    [SerializeField] private PlayerPowerUp powerUpManager;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private BarMove barMove;
    [SerializeField] private HardenedUPGData upgradeData;
    [SerializeField] private SaveManager saveManager;
    public GameObject shieldObject;

    [Header("Stats")]
    [SerializeField] private float duration;
    [SerializeField] private float health;
    [SerializeField] private float impactCost;

    
    private bool suspended = false;

    void Start() {
        int activePath = saveManager.GetActivePath(PowerUpType.Hardened);
        int upgLevel = saveManager.GetUpgradeLevel(PowerUpType.Hardened, activePath);
    
        HardenedUPGEntry upgradeEntry;
        switch (activePath) {
            case 0:
                upgradeEntry = upgradeData.leftPath[upgLevel];
                break;
            case 1:
                upgradeEntry = upgradeData.rightPath[upgLevel];
                break;
            default:
                Debug.LogError("Active Path for Upgrade HARDENED returned incorrect value: " + activePath);
                return;
        }

        this.duration = upgradeEntry.duration;
        this.health = upgradeEntry.health;
        this.impactCost = upgradeEntry.impactCost;
    }

    public void PlaySound() {
        audioSource.clip = Sounds[0];
        audioSource.time = SoundStartTimes[0];
        audioSource.Play();
    }

    public IEnumerator HardenedExpiration() {
        //wait
        powerUpManager.ticker = this.duration;
        while (powerUpManager.GetActivePowerUp() == PowerUpType.Hardened && powerUpManager.ticker > 0) {
            if (!suspended) powerUpManager.BottomBar.value = (powerUpManager.ticker / this.duration);
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

        powerUpManager.ticker = this.duration;
    }

    //TODO: quite messy code, find a better way to do this later
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
