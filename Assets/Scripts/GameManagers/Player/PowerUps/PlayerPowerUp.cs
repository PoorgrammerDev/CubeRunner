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
    [SerializeField] private float colorFadeTime;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Image outline;
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
                    }

                    return false;
                }
                RemovePowerUp();
            }

            this.type = type;
            state = PowerUpState.Standby;
            StartCoroutine(barMove.MoveBarAsync(topBar, 1, 4)); //fill up top bar

            //special actions when picking up PUP ---
            if (type == PowerUpType.Hardened) {
                ChangeUIColor(hardened.Color, colorFadeTime);
                hardened.shieldObject.SetActive(true);
            }
            else if (type == PowerUpType.Blaster) {
                ChangeUIColor(blaster.Color, colorFadeTime);
                blaster.Pickup();
            }
            else if (type == PowerUpType.TimeDilation) {
                ChangeUIColor(timeDilation.Color, colorFadeTime);
            }
            else if (type == PowerUpType.Compress) {
                ChangeUIColor(compression.Color, colorFadeTime);
            }
            return true;
        }
        return false;
    }

    public void RemovePowerUp() {
        if (state != PowerUpState.Empty) {
            state = PowerUpState.Empty;

            //update bar
            ChangeUIColor(defaultColor, colorFadeTime);
            StartCoroutine(barMove.MoveBarAsync(topBar, 0, 4));
            StartCoroutine(barMove.MoveBarAsync(bottomBar, 0, 4));

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

                if (type == PowerUpType.TimeDilation) {
                    StartCoroutine(timeDilation.RunTimeDilation());
                }
                else if (type == PowerUpType.Compress) {
                    StartCoroutine(compression.RunCompression());
                }
                else if (type == PowerUpType.Blaster) {
                    blaster.ShootBlaster();
                }
            }
        }
    }

    void ChangeUIColor (Color color, float duration) {
        outline.CrossFadeColor(color, duration, true, false);
        PUPIcon.CrossFadeColor(color, duration, true, false);
        topBarFill.CrossFadeColor(color, duration, true, false);
        bottomBarFill.CrossFadeColor(color, duration, true, false);
    }
}
