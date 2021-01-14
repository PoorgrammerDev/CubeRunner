using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

enum PowerUpState {
    Empty,
    Standby,
    Active,
}

public class PlayerPowerUp : MonoBehaviour
{
    [SerializeField] private PowerUpState state;
    [SerializeField] private PowerUpType type;
    [SerializeField] private GameValues gameValues;
    [SerializeField] private Volume volume;
    [SerializeField] private GibManager gibManager;
    private float tickDuration = 0.01f;
    private WaitForSeconds tick;
    private WaitForSecondsRealtime tickRT;

    [Header("UI")]
    [SerializeField] private Image PUPIcon;
    [SerializeField] private Slider topBar;
    [SerializeField] private Slider bottomBar;
    
    [Header("Hardened")]
    [SerializeField] private GameObject hardened;

    [Header("Time Dilation")]
    [SerializeField] private float timeDilationDuration;
    [SerializeField] private float timeDilationScale;
    ChromaticAberration chromAb;
    ColorAdjustments colorAdj;

    [Header("Compression")]
    [SerializeField] private float compressionDuration;
    [SerializeField] private float compressionSize;

    [Header("Blaster")]
    [SerializeField] private LineRenderer blasterTracer;
    [SerializeField] private int blasterShots;
    [SerializeField] private int blasterRange;
    [SerializeField] private float blasterCooldown;
    private Vector3 blasterShootDirection = new Vector3(1, 0, 0);
    private int blasterShotsLeft;
    private float blasterCooldownTracker;

    void Start() {
        volume.sharedProfile.TryGet<ChromaticAberration>(out chromAb);
        volume.sharedProfile.TryGet<ColorAdjustments>(out colorAdj);
        ResetTDEffects(false);

        tick = new WaitForSeconds(tickDuration);
        tickRT = new WaitForSecondsRealtime(tickDuration);
    }
    
    public bool AddPowerUp (PowerUpType type) {
        if (state != PowerUpState.Active) {
            if (state == PowerUpState.Standby) {
                if (this.type == type) {
                    //if picking up of same type ---

                    //refill blaster ammo
                    if (type == PowerUpType.Blaster) {
                        blasterShotsLeft = blasterShots;
                        StartCoroutine(MoveBarAsync(topBar, 1, 4));
                    }

                    return false;
                }
                RemovePowerUp();
            }

            this.type = type;
            state = PowerUpState.Standby;
            StartCoroutine(MoveBarAsync(topBar, 1, 4)); //fill up top bar

            //special actions when picking up PUP ---
            if (type == PowerUpType.Hardened) {
                hardened.SetActive(true);
            }
            else if (type == PowerUpType.Blaster) {
                blasterShotsLeft = blasterShots;   
                StartCoroutine(MoveBarAsync(bottomBar, 1, 4));

                blasterCooldownTracker = 0;
                StartCoroutine(BlasterCooldown());
            }
            return true;
        }
        return false;
    }

    public void RemovePowerUp() {
        if (state != PowerUpState.Empty) {
            state = PowerUpState.Empty;
            StartCoroutine(MoveBarAsync(topBar, 0, 4));
            StartCoroutine(MoveBarAsync(bottomBar, 0, 4));

            //special actions when removing PUP
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
                PowerUpType? type = GetActivePowerUp();

                if (type == PowerUpType.TimeDilation) {
                    StartCoroutine(RunTimeDilation());
                }
                else if (type == PowerUpType.Compress) {
                    StartCoroutine(RunCompression());
                }
                else if (type == PowerUpType.Blaster) {
                    ShootBlaster();
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
        while (t <= 1) {
            t += 8 * Time.deltaTime;
            chromAb.intensity.SetValue(new NoInterpClampedFloatParameter(Mathf.Lerp(0, 1, t), 0, 1, true));
            colorAdj.saturation.SetValue(new ClampedFloatParameter(Mathf.Lerp(0, -45, t), -100, 100, true));
            yield return null;
        }

        t = 0;

        //wait
        for (float i = 0; i < timeDilationDuration; i+= tickDuration) {
            topBar.value = 1 - (i / timeDilationDuration);
            yield return tickRT;
        }

        //speed back up and get rid of effects
        while (t <= 1) {
            t += 2 * Time.deltaTime;
            chromAb.intensity.SetValue(new NoInterpClampedFloatParameter(Mathf.Lerp(1, 0, t), 0, 1, true));
            colorAdj.saturation.SetValue(new ClampedFloatParameter(Mathf.Lerp(-45, 0, t), -100, 100, true));
            Time.timeScale = Mathf.Lerp(timeDilationScale, 1, t);
            yield return null;
        }

        chromAb.active = false;
        colorAdj.active = false;
        RemovePowerUp();
    }

    public void ResetTDEffects(bool stopCoroutine) {
        if (stopCoroutine) StopCoroutine(RunTimeDilation());

        chromAb.active = false;
        colorAdj.active = false;

        //hard clamp to default values
        Time.timeScale = 1f;
        chromAb.intensity.SetValue(new NoInterpClampedFloatParameter(0, 0, 1, true));
        colorAdj.saturation.SetValue(new ClampedFloatParameter(0, -100, 100, true));
    }

    //COMPRESSION---------------------------------------

    IEnumerator RunCompression() {
        state = PowerUpState.Active;
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
        for (float i = 0; i < compressionDuration; i+= tickDuration) {
            topBar.value = 1 - (i / compressionDuration);
            yield return tick;
        }
        

        //return to original size
        while (t >= 0) {
            t -= 4 * Time.deltaTime;
            scale.x = scale.y = scale.z = Mathf.Lerp(originalScale, compressionSize, t);
            transform.localScale = scale;

            position = transform.position;
            position.y = Mathf.Lerp(originalY, newY, t);
            transform.position = position;
            yield return null;
        }
        RemovePowerUp();
    }

    //BLASTER-------------------------------------------
    void ShootBlaster() {
        if (blasterCooldownTracker <= 0) {
            if (blasterShotsLeft > 0) {
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

                        gibManager.Activate(hitObject.position, hitObject.localScale, true, true);
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
                StartCoroutine(MoveBarAsync(topBar, targetValue, 4));
            }

            //if ammo is empty, remove
            if (blasterShotsLeft == 0) {
                RemovePowerUp();
            }
        }
    }

    IEnumerator BlasterCooldown() {
        while (GetActivePowerUp() == PowerUpType.Blaster) {
            if (blasterCooldownTracker > 0) {
                blasterCooldownTracker -= tickDuration;
                bottomBar.value = 1f - (blasterCooldownTracker / blasterCooldown);
            }
            yield return tick;
        }
    }

    IEnumerator MoveBarAsync(Slider bar, float targetValue, float speed) {
        float t = 0f;
        float currentValue = bar.value;
        while (t <= 1) {
            t += speed * Time.deltaTime;
            bar.value = Mathf.Lerp(currentValue, targetValue, t);
            yield return null;
        }
    }
}
