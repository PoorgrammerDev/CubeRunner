using System.Collections;
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

    //Power up classes
    [Header("Power Up Classes")]
    [SerializeField] private Blaster blaster;
    [SerializeField] private TimeDilation timeDilation;
    [SerializeField] private Compression compression;
    [SerializeField] private Hardened hardened;
    [SerializeField] private Guidelines guidelines;
    [SerializeField] private Decimate decimate;
    [SerializeField] private Align align;

    [Header("Ticking")]
    [SerializeField] private float tickRate;
     public float ticker = 0;

    [Header("UI")]
    [SerializeField] private Animator PowerUpHUDAnimator;
    private AspectRatioFitter PUPHUDXYFitter;
    private int activeFitterTasks = 0;
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
        PUPHUDXYFitter = PowerUpHUDAnimator.GetComponent<AspectRatioFitter>();

        PowerUpTypeToClass = new Dictionary<PowerUpType, AbstractPowerUp>();
        PowerUpTypeToClass.Add(PowerUpType.Blaster, blaster);
        PowerUpTypeToClass.Add(PowerUpType.TimeDilation, timeDilation);
        PowerUpTypeToClass.Add(PowerUpType.Compression, compression);
        PowerUpTypeToClass.Add(PowerUpType.Hardened, hardened);
        PowerUpTypeToClass.Add(PowerUpType.Guidelines, guidelines);
        PowerUpTypeToClass.Add(PowerUpType.Decimate, decimate);
        PowerUpTypeToClass.Add(PowerUpType.Align, align);
    }
    
    public bool AddPowerUp (PowerUpType type) {
        if (state != PowerUpState.Active) {
            if (state == PowerUpState.Standby) {
                if (this.type == type) {

                    //if picking up of same type ---
                    switch (type) {
                        //refill blaster ammo
                        case PowerUpType.Blaster:
                            blaster.RefillAmmo();
                            StartCoroutine(barMove.MoveBarAsync(topBar, 1, 4));
                            return true;

                        //recharge shield time
                        case PowerUpType.Hardened:
                            hardened.ResetExpiry();
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
                audioSource.time = 0;
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
                switch (type) {
                    case PowerUpType.Blaster:
                        blaster.Pickup();
                        break;

                    case PowerUpType.Hardened:
                        hardened.shieldObject.SetActive(true);
                        StartCoroutine(hardened.HardenedExpiration());
                        break;
                    case PowerUpType.Decimate:
                        decimate.RunDecimate();
                        break;
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
            switch (type) {
                case PowerUpType.Hardened:
                    hardened.shieldObject.SetActive(false);
                    break;
                case PowerUpType.Guidelines:
                    StartCoroutine(guidelines.StopSound(false));
                    break;
                case PowerUpType.Align:
                    align.targetParentObj.gameObject.SetActive(false);
                    align.leftMoveArrow.SetActive(false);
                    align.rightMoveArrow.SetActive(false);
                    break;
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

            #if UNITY_ANDROID || UNITY_IOS
                if (ticker < 0 && GetActivePowerUp() == PowerUpType.Blaster) {
                    //fade in pup button
                    mobilePUPButton.ResetTrigger(TagHolder.ANIM_FADE_OUT);
                    mobilePUPButton.SetTrigger(TagHolder.ANIM_FADE_IN);
                }
            #endif
        }

        //TODO: THIS IS A HACKY WORKAROUND
        if (activeFitterTasks > 0) {
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
            
            //disable mobile power up button
            #if UNITY_ANDROID || UNITY_IOS
                bool disableButton = false;
            #endif

            //detect power ups activation
            switch (type) {
                case PowerUpType.TimeDilation:
                    StartCoroutine(timeDilation.RunTimeDilation());
                    playSound = timeDilation;

                    //disable power up button if mobile
                    #if UNITY_ANDROID || UNITY_IOS
                        disableButton = true;
                    #endif
                    break;
                case PowerUpType.Compression:
                    StartCoroutine(compression.RunCompression());
                    playSound = compression;

                    //disable power up button if mobile
                    #if UNITY_ANDROID || UNITY_IOS
                        disableButton = true;
                    #endif
                    break;
                case PowerUpType.Blaster:
                    if (blaster.ShootBlaster()) {
                        playSound = blaster;
                    }

                    //disable power up button if mobile
                    #if UNITY_ANDROID || UNITY_IOS
                        disableButton = true;
                    #endif
                    break;
                case PowerUpType.Guidelines:
                    StartCoroutine(guidelines.RunGuidelines());

                    //disable power up button if mobile
                    #if UNITY_ANDROID || UNITY_IOS
                        disableButton = true;
                    #endif
                    break;
                case PowerUpType.Align:
                    StartCoroutine(align.RunAlign());

                    //disable power up button if mobile
                    #if UNITY_ANDROID || UNITY_IOS
                        disableButton = true;
                    #endif
                    break;
            }

            //disable power up button
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
        activeFitterTasks++;
        while (t <= 1) {
            t += 2.5f * Time.deltaTime;
            PUPHUDXYFitter.aspectRatio = open ?  Mathf.Lerp(1, 4, t) : Mathf.Lerp(4, 1, t);
            yield return null;
        }
        activeFitterTasks--;
    }

    public void StopSound() {
        audioSource.Stop();
    }
}
