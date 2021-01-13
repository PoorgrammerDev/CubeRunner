using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

enum PowerUpState {
    Empty,
    Standby,
    Active,
}

public class PlayerPowerUp : MonoBehaviour
{
    [SerializeField] private GameValues gameValues;
    [SerializeField] private Volume volume;
    [SerializeField] private PowerUpState state;
    [SerializeField] private PowerUpType type;
    [SerializeField] private GameObject blaster;
    [SerializeField] private GameObject hardened;
    [SerializeField] private GibManager gibManager;

    [Header("Time Dilation")]
    [SerializeField] private float timeDilationDuration;
    [SerializeField] private float timeDilationScale;
    ChromaticAberration chromAb;
    ColorAdjustments colorAdj;

    [Header("Compression")]
    [SerializeField] private float compressionDuration;
    [SerializeField] private float compressionSize;

    [Header("Blaster")]
    [SerializeField] private int blasterShots;
    [SerializeField] private int blasterRange;
    private Vector3 blasterShootDirection = new Vector3(1, 0, 0);
    private int blasterShotsLeft;

    void Start() {
        volume.sharedProfile.TryGet<ChromaticAberration>(out chromAb);
        volume.sharedProfile.TryGet<ColorAdjustments>(out colorAdj);
        ResetTDEffects(false);
    }
    
    public bool AddPowerUp (PowerUpType type) {
        if (state != PowerUpState.Active) {
            if (state == PowerUpState.Standby) {
                if (this.type == type) return false;
                RemovePowerUp();
            }

            this.type = type;
            state = PowerUpState.Standby;

            //special actions when picking up PUP
            if (type == PowerUpType.Hardened) {
                hardened.SetActive(true);
            }
            else if (type == PowerUpType.Blaster) {
                blasterShotsLeft = blasterShots;
            }
            return true;
        }
        return false;
    }

    public void RemovePowerUp() {
        if (state != PowerUpState.Empty) {
            state = PowerUpState.Empty;

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
                PowerUpType type = this.type;
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
        while (t < 1) {
            chromAb.intensity.SetValue(new NoInterpClampedFloatParameter(Mathf.Lerp(0, 1, t), 0, 1, true));
            colorAdj.saturation.SetValue(new ClampedFloatParameter(Mathf.Lerp(0, -45, t), -100, 100, true));
            t += 8 * Time.deltaTime;
            yield return null;
        }

        t = 0;

        //wait
        yield return new WaitForSeconds(timeDilationDuration);

        //speed back up and get rid of effects
        while (t < 1) {
            chromAb.intensity.SetValue(new NoInterpClampedFloatParameter(Mathf.Lerp(1, 0, t), 0, 1, true));
            colorAdj.saturation.SetValue(new ClampedFloatParameter(Mathf.Lerp(-45, 0, t), -100, 100, true));
            Time.timeScale = Mathf.Lerp(timeDilationScale, 1, t);
            t += 2 * Time.deltaTime;
            yield return null;
        }

        RemovePowerUp();
        ResetTDEffects(false);
    }

    public void ResetTDEffects(bool stopCoroutine) {
        if (stopCoroutine) StopCoroutine(RunTimeDilation());

        //hard clamp values back
        Time.timeScale = 1f;
        chromAb.intensity.SetValue(new NoInterpClampedFloatParameter(0, 0, 1, true));
        colorAdj.saturation.SetValue(new ClampedFloatParameter(0, -100, 100, true));
        chromAb.active = false;
        colorAdj.active = false;
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
        while (t < 1) {
            scale.x = scale.y = scale.z = Mathf.Lerp(originalScale, compressionSize, t);
            transform.localScale = scale;

            position = transform.position;
            position.y = Mathf.Lerp(originalY, newY, t);
            transform.position = position;

            t += 4 * Time.deltaTime;
            yield return null;
        }

        //wait
        yield return new WaitForSeconds(compressionDuration);

        //return to original size
        while (t > 0) {
            scale.x = scale.y = scale.z = Mathf.Lerp(originalScale, compressionSize, t);
            transform.localScale = scale;

            position = transform.position;
            position.y = Mathf.Lerp(originalY, newY, t);
            transform.position = position;

            t -= 4 * Time.deltaTime;
            yield return null;
        }

        //hard clamp values
        scale.x = scale.y = scale.z = originalScale;
        transform.localScale = scale;
        
        position = transform.position;
        position.y = originalY;
        transform.position = position;
        RemovePowerUp();
    }

    //BLASTER-------------------------------------------
    void ShootBlaster() {
        if (blasterShotsLeft > 0) {
            blasterShotsLeft--;
            
            //front posiiton of cube
            Vector3 position = transform.position;
            position.x += 0.51f;
            
            //shoot out and break obstacle if hit
            RaycastHit hit;
            if (Physics.Raycast(position, blasterShootDirection, out hit, blasterRange)) {
                Transform hitObject = hit.transform;
                if (hitObject.CompareTag(TagHolder.OBSTACLE_TAG)) {
                    hitObject.GetComponent<Renderer>().enabled = false;
                    hitObject.GetComponent<Collider>().enabled = false;

                    gibManager.Activate(hitObject.position, hitObject.localScale, true, true);
                }
            }
        }

        //if ammo is empty, remove
        if (blasterShotsLeft <= 0) {
            RemovePowerUp();
        }
    }
}
