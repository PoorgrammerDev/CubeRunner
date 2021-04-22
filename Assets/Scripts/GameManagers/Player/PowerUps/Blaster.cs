using System.Collections;
using UnityEngine;

public class Blaster : AbstractPowerUp {
    [Header("References")]
    [SerializeField] private PlayerPowerUp powerUpManager;
    [SerializeField] private GameValues gameValues;
    [SerializeField] private BarMove barMove;
    [SerializeField] private GibManager gibManager;
    [SerializeField] private LineRenderer blasterTracer;
    [SerializeField] private AudioSource obstacleHitSource;
    [SerializeField] private BlasterUPGData upgradeData;
    [SerializeField] private SaveManager saveManager;

    //BLASTER STATS --- --- --- --- ---
    private float cooldown;
    private int maxAmmo;
    private float range;
    private int piercing;

    //OTHER FIELDS --- --- --- --- ---
    private Vector3 blasterShootDirection = new Vector3(1, 0, 0);
    private int remainingAmmo;

    void Start() {
        int activePath = saveManager.GetActivePath(PowerUpType.Blaster);
        int upgLevel = saveManager.GetUpgradeLevel(PowerUpType.Blaster, activePath);

        BlasterUPGEntry upgradeEntry;
        switch (activePath) {
            case 0:
                upgradeEntry = upgradeData.leftPath[upgLevel];
                break;
            case 1:
                upgradeEntry = upgradeData.rightPath[upgLevel];
                break;
            default:
                Debug.LogError("Active Path for Upgrade BLASTER returned incorrect value: " + activePath);
                return;
        }

        this.maxAmmo = upgradeEntry.ammo;
        this.range = upgradeEntry.range;
        this.cooldown = upgradeEntry.cooldown;
        this.piercing = upgradeEntry.piercing;
    }

    public void Pickup() {
        this.remainingAmmo = this.maxAmmo;   
        StartCoroutine(barMove.MoveBarAsync(powerUpManager.BottomBar, 1, 4));
        StartCoroutine(CooldownBar());
    }

    public void RefillAmmo() {
        this.remainingAmmo = this.maxAmmo;
    }


    public bool ShootBlaster() {
        if (powerUpManager.ticker <= 0) {
            bool result = false;
            if (this.remainingAmmo > 0) {
                result = true;
                powerUpManager.ticker = this.cooldown;

                //front position of cube
                Vector3 position = transform.position;
                position.x += 0.51f;

                //shoot out and break obstacle if hit
                RecursivePiercingShot(position, this.range, this.piercing);

                //visual effect
                //TODO: this currently always draws the line the entire range, not counting if it hits obstacles and stops there
                Vector3 tracerPos = blasterTracer.GetPosition(0);
                tracerPos.z = transform.position.z;
                blasterTracer.SetPosition(0, tracerPos);

                tracerPos = blasterTracer.GetPosition(1);
                tracerPos.x = this.range;
                tracerPos.z = transform.position.z;
                blasterTracer.SetPosition(1, tracerPos);

                blasterTracer.gameObject.SetActive(true);

                //update UI
                float targetValue = (float) (--this.remainingAmmo) / (float) this.maxAmmo;
                StartCoroutine(barMove.MoveBarAsync(powerUpManager.TopBar, targetValue, 4));
            }

            //if ammo is empty, remove
            if (this.remainingAmmo == 0) {
                powerUpManager.RemovePowerUp();
            }

            return result;
        }
        return false;
    }

    void RecursivePiercingShot(Vector3 startingPosition, float remainingRange, float remainingPierces) {
        RaycastHit hit;

        if (Physics.Raycast(startingPosition, blasterShootDirection, out hit, remainingRange)) {
            Transform hitObject = hit.transform;
            if (hitObject.CompareTag(TagHolder.OBSTACLE_TAG)) {
                //obstacle break sfx
                obstacleHitSource.clip = Sounds[1];
                obstacleHitSource.time = SoundStartTimes[1];
                obstacleHitSource.Play();

                //obstacle break vfx
                hitObject.gameObject.SetActive(false);
                if (gameValues.Divide != 0) {
                    gibManager.Activate(hitObject.position, hitObject.localScale, true, true);
                }

                remainingPierces--;
                remainingRange -= (hitObject.position.x - startingPosition.x);

                if (remainingRange > 0 && remainingPierces > 0) {
                    RecursivePiercingShot(hitObject.position, remainingRange, remainingPierces);
                }
            }
        }
    }

    public IEnumerator CooldownBar() {
        powerUpManager.ticker = 0;
        while (powerUpManager.GetActivePowerUp() == PowerUpType.Blaster) {
            powerUpManager.BottomBar.value = 1f - (powerUpManager.ticker / this.cooldown);
            yield return null;
        }
    }
}