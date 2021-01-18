using System.Collections;
using UnityEngine;

public class Blaster : AbstractPowerUp {
    [SerializeField] private GameValues gameValues;
    [SerializeField] private BarMove barMove;
    [SerializeField] private GibManager gibManager;
    [SerializeField] private LineRenderer blasterTracer;
    [SerializeField] private AudioSource obstacleHitSource;

    [SerializeField] private int blasterShots;

    [SerializeField] private int blasterRange;
    [SerializeField] private float blasterCooldown;
    private PlayerPowerUp powerUpManager;
    private Vector3 blasterShootDirection = new Vector3(1, 0, 0);
    
    private int blasterShotsLeft;

    private float blasterCooldownTracker;

    void Start() {
        powerUpManager = GetComponent<PlayerPowerUp>();
    }

    public void Pickup() {
        blasterShotsLeft = blasterShots;   
        StartCoroutine(barMove.MoveBarAsync(powerUpManager.BottomBar, 1, 4));

        blasterCooldownTracker = 0;
        StartCoroutine(BlasterCooldown());
    }

    public void RefillAmmo() {
        blasterShotsLeft = blasterShots;
    }


    public bool ShootBlaster() {
        if (blasterCooldownTracker <= 0) {
            bool result = false;
            if (blasterShotsLeft > 0) {
                result = true;
                blasterCooldownTracker = blasterCooldown;

                //front posiiton of cube
                Vector3 position = transform.position;
                position.x += 0.51f;


                float x = blasterRange;
                //shoot out and break obstacle if hit
                RaycastHit hit;
                if (Physics.Raycast(position, blasterShootDirection, out hit, blasterRange)) {
                    Transform hitObject = hit.transform;
                    if (hitObject.CompareTag(TagHolder.OBSTACLE_TAG)) {
                        x = hitObject.position.x;
                        hitObject.gameObject.SetActive(false);

                        //sound
                        obstacleHitSource.clip = Sounds[1];
                        obstacleHitSource.time = SoundStartTimes[1];
                        obstacleHitSource.Play();

                        if (gameValues.Divide != 0) {
                            gibManager.Activate(hitObject.position, hitObject.localScale, true, true);
                        }
                    }
                }

                //visual effect
                Vector3 tracerPos = blasterTracer.GetPosition(0);
                tracerPos.z = transform.position.z;
                blasterTracer.SetPosition(0, tracerPos);

                tracerPos = blasterTracer.GetPosition(1);
                tracerPos.x = x;
                tracerPos.z = transform.position.z;
                blasterTracer.SetPosition(1, tracerPos);

                blasterTracer.gameObject.SetActive(true);

                //update UI
                float targetValue = (float) (--blasterShotsLeft) / (float) blasterShots;
                StartCoroutine(barMove.MoveBarAsync(powerUpManager.TopBar, targetValue, 4));
            }

            //if ammo is empty, remove
            if (blasterShotsLeft == 0) {
                powerUpManager.RemovePowerUp();
            }

            return result;
        }
        return false;
    }

    public IEnumerator BlasterCooldown() {
        while (powerUpManager.GetActivePowerUp() == PowerUpType.Blaster) {
            if (blasterCooldownTracker > 0) {
                blasterCooldownTracker -= powerUpManager.TickDuration;
                powerUpManager.BottomBar.value = 1f - (blasterCooldownTracker / blasterCooldown);
            }
            yield return powerUpManager.Tick;
        }
    }
}