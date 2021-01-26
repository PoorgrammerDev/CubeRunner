﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PowerUpState {
    Empty,
    Standby,
    Active,
}

public class PlayerPowerUp : MonoBehaviour
{
    [SerializeField] private PowerUpState state;
     public PowerUpState State { get => state; set => state = value; }
    [SerializeField] private PowerUpType type;
    [SerializeField] private GameValues gameValues;
    [SerializeField] private BarMove barMove;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pickupPUPSound;
    private Dictionary<PowerUpType, AbstractPowerUp> PowerUpTypeToClass;
    private AbstractPowerUp ActivePowerUpClass;
    private Blaster blaster;
    private TimeDilation timeDilation;
    private Compression compression;
    private Hardened hardened;

    [Header("Ticking")]
    [SerializeField] private float tickRate;
    [HideInInspector] public float ticker = 0;

    [Header("UI")]
    [SerializeField] private Animator PowerUpHUDAnimator;
    private AspectRatioFitter PUPHUDXYFitter;
    private bool aspectFitterWorking = false;
    private bool fitterWorkVar = false;
    [SerializeField] private float colorFadeTime;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Image outline;
    [SerializeField] private Image PUPIconBG;
    [SerializeField] private Image PUPIcon;

    [SerializeField] private Slider topBar;
    [SerializeField] private Image topBarFill;
    public Slider TopBar => topBar;

    [SerializeField] private Slider bottomBar;
    [SerializeField] private Image bottomBarFill;
    public Slider BottomBar => bottomBar;

    //mobile
    [SerializeField] private Animator mobilePUPButton;

    void Start() {
        //get individual PUP classes
        blaster = GetComponent<Blaster>();
        timeDilation = GetComponent<TimeDilation>();
        compression = GetComponent<Compression>();
        hardened = GetComponent<Hardened>();

        PUPHUDXYFitter = PowerUpHUDAnimator.GetComponent<AspectRatioFitter>();

        PowerUpTypeToClass = new Dictionary<PowerUpType, AbstractPowerUp>();
        PowerUpTypeToClass.Add(PowerUpType.Blaster, blaster);
        PowerUpTypeToClass.Add(PowerUpType.TimeDilation, timeDilation);
        PowerUpTypeToClass.Add(PowerUpType.Compress, compression);
        PowerUpTypeToClass.Add(PowerUpType.Hardened, hardened);
    }
    
    public bool AddPowerUp (PowerUpType type) {
        if (state != PowerUpState.Active) {
            if (state == PowerUpState.Standby) {
                if (this.type == type) {
                    //if picking up of same type ---

                    //refill blaster ammo
                    if (type == PowerUpType.Blaster) {
                        blaster.RefillAmmo();
                        StartCoroutine(barMove.MoveBarAsync(topBar, 1, 4));
                        return true;
                    }

                    return false;
                }
                RemovePowerUp();
            }

            if (PowerUpTypeToClass.TryGetValue(type, out ActivePowerUpClass)) {
                this.type = type;
                state = PowerUpState.Standby;

                //open bar
                PowerUpHUDAnimator.ResetTrigger(TagHolder.PUP_HUD_CLOSE_TRIGGER);
                PowerUpHUDAnimator.SetTrigger(TagHolder.PUP_HUD_OPEN_TRIGGER);
                StartCoroutine(TogglePUPHUD(true));

                StartCoroutine(barMove.MoveBarAsync(topBar, 1, 4)); //fill up top bar

                //bar color
                ChangeUIColor(ActivePowerUpClass.Color, colorFadeTime);

                //pickup sound
                audioSource.clip = pickupPUPSound;
                audioSource.Play();

                #if UNITY_ANDROID || UNITY_IOS
                    if (type != PowerUpType.Hardened) {
                        //fade in pup button
                        mobilePUPButton.ResetTrigger(TagHolder.ANIM_FADE_OUT);
                        mobilePUPButton.SetTrigger(TagHolder.ANIM_FADE_IN);
                    }
                #endif

                //change icon
                PUPIcon.enabled = true;
                PUPIcon.sprite = ActivePowerUpClass.Sprite;

                //special actions when picking up PUP ---
                if (type == PowerUpType.Hardened) {
                    hardened.shieldObject.SetActive(true);
                }
                else if (type == PowerUpType.Blaster) {
                    blaster.Pickup();
                }
                return true;
            }
        }
        return false;
    }

    public void RemovePowerUp() {
        if (state != PowerUpState.Empty) {
            state = PowerUpState.Empty;

            //close bar
            PowerUpHUDAnimator.ResetTrigger(TagHolder.PUP_HUD_OPEN_TRIGGER);
            PowerUpHUDAnimator.SetTrigger(TagHolder.PUP_HUD_CLOSE_TRIGGER);
            StartCoroutine(TogglePUPHUD(false));

            #if UNITY_ANDROID || UNITY_IOS
                //fade out pup button
                mobilePUPButton.ResetTrigger(TagHolder.ANIM_FADE_IN);
                mobilePUPButton.SetTrigger(TagHolder.ANIM_FADE_OUT);
            #endif

            //update bar
            ChangeUIColor(defaultColor, colorFadeTime);
            StartCoroutine(barMove.MoveBarAsync(topBar, 0, 4));
            StartCoroutine(barMove.MoveBarAsync(bottomBar, 0, 4));

            //remove icon
            PUPIcon.enabled = false;

            //special actions when removing PUP
            if (type == PowerUpType.Hardened) {
                hardened.shieldObject.SetActive(false);
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

        if (ticker > 0) {
            ticker -= tickRate * Time.deltaTime;
        }

        //THIS IS A HACKY WORKAROUND
        if (aspectFitterWorking) {
            PUPHUDXYFitter.enabled = fitterWorkVar;
            fitterWorkVar = !fitterWorkVar;
        }

        #if UNITY_STANDALONE || UNITY_WEBGL
            if (Input.GetKeyDown(KeyCode.F)) {
                ClickedPUPUse();
            }
        #endif
    }
    

    public void ClickedPUPUse () {
        if (!gameValues.GameActive) return;

        if (state == PowerUpState.Standby) {
            PowerUpType? type = GetActivePowerUp();
            AbstractPowerUp playSound = null;
            
            #if UNITY_ANDROID || UNITY_IOS
                bool disableButton = false;
            #endif

            //detect power ups activation
            if (type == PowerUpType.TimeDilation) {
                StartCoroutine(timeDilation.RunTimeDilation());
                playSound = timeDilation;

                #if UNITY_ANDROID || UNITY_IOS
                    disableButton = true;
                #endif
            }
            else if (type == PowerUpType.Compress) {
                StartCoroutine(compression.RunCompression());
                playSound = compression;
                
                #if UNITY_ANDROID || UNITY_IOS
                    disableButton = true;
                #endif
            }
            else if (type == PowerUpType.Blaster) {
                if (blaster.ShootBlaster()) {
                    playSound = blaster;
                }
            }

            #if UNITY_ANDROID || UNITY_IOS
                if (disableButton) {
                    //fade out pup button
                    mobilePUPButton.ResetTrigger(TagHolder.ANIM_FADE_IN);
                    mobilePUPButton.SetTrigger(TagHolder.ANIM_FADE_OUT);
                }
            #endif


            //playing sound
            if (playSound != null) {
                audioSource.clip = playSound.Sounds[0];
                audioSource.time = playSound.SoundStartTimes[0];
                audioSource.Play();
            }
        }
    }

    void ChangeUIColor (Color color, float duration) {
        outline.CrossFadeColor(color, duration, true, false);
        PUPIconBG.CrossFadeColor(color, duration, true, false);
        topBarFill.CrossFadeColor(color, duration, true, false);
        bottomBarFill.CrossFadeColor(color, duration, true, false);
    }

    IEnumerator TogglePUPHUD(bool open) {
        float t = 0f;
        aspectFitterWorking = true;
        while (t <= 1) {
            t += 2.5f * Time.deltaTime;
            PUPHUDXYFitter.aspectRatio = open ?  Mathf.Lerp(1, 4, t) : Mathf.Lerp(4, 1, t);
            yield return null;
        }
        aspectFitterWorking = false;
    }
}
