using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hardened : AbstractPowerUp
{
    [Header("References")]
    [SerializeField] private PlayerPowerUp powerUpManager;
    [SerializeField] private GameValues gameValues;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private BarMove barMove;
    [SerializeField] private HardenedUPGData upgradeData;
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private GibManager obstacleGibManager;
    public GameObject shieldObject;

    [Header("Stats")]
    [SerializeField] private int duration;
    [SerializeField] private int maxHealth;
    [SerializeField] private int impactCost;

    private int health;

    [Header("Other Options")]
    public LayerMask obstacleLayer;

    
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
        this.maxHealth = upgradeEntry.health;
        this.impactCost = upgradeEntry.impactCost;
    }
    
    public IEnumerator HardenedExpiration() {
        //set health to max
        this.health = this.maxHealth;

        //wait
        powerUpManager.ticker = this.duration;
        while (powerUpManager.GetActivePowerUp() == PowerUpType.Hardened && powerUpManager.ticker > 0) {
            if (!suspended) {
                powerUpManager.TopBar.value = ((float) this.health / (float) this.maxHealth);
                powerUpManager.BottomBar.value = (powerUpManager.ticker / this.duration);
            }
            yield return null;
        }

        //expire hardened
        if (powerUpManager.GetActivePowerUp() == PowerUpType.Hardened) powerUpManager.RemovePowerUp();
    }

    public void Impact(Transform playerTransform) {
        //finds all colliding obstacles
        Collider[] allObstacles = Physics.OverlapBox(playerTransform.position, playerTransform.localScale / 2f, Quaternion.identity, obstacleLayer, QueryTriggerInteraction.Collide);

        //destroys all obstacles colliding with
        foreach (Collider obstacle in allObstacles) {
            //smashing obstacle
            if (gameValues.Divide != 0) {
                obstacleGibManager.Activate(obstacle.transform.position, obstacle.transform.localScale, true, true);
            }

            obstacle.gameObject.SetActive(false);
        }

        //plays sound and decrements shield health
        PlaySound();
        DecrementHealth();
    }

    private void DecrementHealth() {
        this.health--;

        //if health reaches 0, remove PUP
        if (this.health <= 0) {
            powerUpManager.RemovePowerUp();
            return;
        }

        //take into account impact cost
        powerUpManager.ticker -= (this.duration * (this.impactCost / 100.0f));
    }

    private void PlaySound() {
        audioSource.clip = Sounds[0];
        audioSource.time = SoundStartTimes[0];
        audioSource.Play();
    }

    public void ResetExpiry() {
        //suspends regular bar animation, 
        suspended = true;
        StartCoroutine(barMove.MoveBarAsync(powerUpManager.TopBar, 1, 4));
        StartCoroutine(barMove.MoveBarAsync(powerUpManager.BottomBar, 1, 4));
        StartCoroutine(DelayResume(4));

        powerUpManager.ticker = this.duration;
        this.health = this.maxHealth;
    }

    //TODO: quite messy code, find a better way to do this later
    private IEnumerator DelayResume(float speed) {
        float t = 0f;
        while (t <= 1f) {
            t += speed * Time.deltaTime;
            yield return null;
        }

        suspended = false;
    }

    //NOTE: The actual shield mechanism is handled in PlayerCollision.cs
}
