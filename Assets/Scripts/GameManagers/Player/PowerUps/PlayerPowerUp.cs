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
    private Dictionary<PowerUpType, AbstractPowerUp> PowerUpTypeToClass;
    private AbstractPowerUp ActivePowerUpClass;
    private Blaster blaster;
    private TimeDilation timeDilation;
    private Compression compression;
    private Hardened hardened;

    [Header("Ticking")]
    private float tickDuration = 0.01f;
    public float TickDuration => tickDuration;

    private WaitForSeconds tick;
    public WaitForSeconds Tick => tick;

    private WaitForSecondsRealtime tickRT;
    public WaitForSecondsRealtime TickRT => tickRT;

    [Header("UI")]
    [SerializeField] private Animator PowerUpHUDAnimator;
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

    void Start() {
        //get individual PUP classes
        blaster = GetComponent<Blaster>();
        timeDilation = GetComponent<TimeDilation>();
        compression = GetComponent<Compression>();
        hardened = GetComponent<Hardened>();

        PowerUpTypeToClass = new Dictionary<PowerUpType, AbstractPowerUp>();
        PowerUpTypeToClass.Add(PowerUpType.Blaster, blaster);
        PowerUpTypeToClass.Add(PowerUpType.TimeDilation, timeDilation);
        PowerUpTypeToClass.Add(PowerUpType.Compress, compression);
        PowerUpTypeToClass.Add(PowerUpType.Hardened, hardened);

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

                StartCoroutine(barMove.MoveBarAsync(topBar, 1, 4)); //fill up top bar

                //bar color
                ChangeUIColor(ActivePowerUpClass.Color, colorFadeTime);

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

        if (Input.GetKeyDown(KeyCode.F)) {
            if (state == PowerUpState.Standby) {
                PowerUpType? type = GetActivePowerUp();
                AbstractPowerUp playSound = null;

                //detect power ups activation
                if (type == PowerUpType.TimeDilation) {
                    StartCoroutine(timeDilation.RunTimeDilation());
                    playSound = timeDilation;
                }
                else if (type == PowerUpType.Compress) {
                    StartCoroutine(compression.RunCompression());
                    playSound = compression;
                }
                else if (type == PowerUpType.Blaster) {
                    if (blaster.ShootBlaster()) {
                        playSound = blaster;
                    }
                }


                //playing sound
                if (playSound != null) {
                    audioSource.clip = playSound.Sounds[0];
                    audioSource.time = playSound.SoundStartTimes[0];
                    audioSource.Play();
                }
            }
        }
    }

    void ChangeUIColor (Color color, float duration) {
        outline.CrossFadeColor(color, duration, true, false);
        PUPIconBG.CrossFadeColor(color, duration, true, false);
        topBarFill.CrossFadeColor(color, duration, true, false);
        bottomBarFill.CrossFadeColor(color, duration, true, false);
    }
}
