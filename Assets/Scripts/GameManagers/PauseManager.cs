using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private Animator pauseMenu;
    [SerializeField] private Animator GameHUD;
    [SerializeField] private EndGame endGame;
    [SerializeField] private MusicManager musicManager;
    private SFXVolumeManager[] sfxManagers;

    [SerializeField] private Button pauseButton;

    public bool paused = false;

    private float currentTimeScale = -1;
    private bool musicWasPlaying = false;

    void Start() {
        sfxManagers = FindObjectsOfType<SFXVolumeManager>();
    }


    //escape button for pause/unpause
    public void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (paused) {
                if (pauseMenu.gameObject.activeInHierarchy) Resume();
            }
            else if (pauseButton.interactable) {
                Pause();
            }
        }
    }

    public void Pause() {
        if (pauseMenu.gameObject.activeInHierarchy) return;

        //set paused state
        paused = true;

        //record time scale, freeze time, remove hud
        currentTimeScale = Time.timeScale;
        GameHUD.ResetTrigger(TagHolder.HUD_ENTER_TRIGGER);
        GameHUD.SetTrigger(TagHolder.HUD_EXIT_TRIGGER);
        Time.timeScale = 0f;

        //pause music, sfx
        musicWasPlaying = !musicManager.Stopped;
        if (musicWasPlaying) musicManager.Pause();
        foreach (SFXVolumeManager sfxMng in sfxManagers) {
            sfxMng.Pause();
        }

        //menu enters
        pauseMenu.gameObject.SetActive(true);
    }

    public void Resume() {
        if (!pauseMenu.gameObject.activeInHierarchy) return;

        //set pause state
        paused = false;

        //hud re-enters
        GameHUD.ResetTrigger(TagHolder.HUD_EXIT_TRIGGER);
        GameHUD.SetTrigger(TagHolder.HUD_ENTER_TRIGGER);
        
        //resume time
        Time.timeScale = (currentTimeScale != -1) ? currentTimeScale : 1;
        currentTimeScale = -1;

        //music, sfx
        if (musicWasPlaying) musicManager.Resume();
        musicWasPlaying = false;
        foreach (SFXVolumeManager sfxMng in sfxManagers) {
            sfxMng.Resume();
        }

        //remove pause menu
        pauseMenu.Play(TagHolder.HUD_EXIT_TRIGGER);
    }

    public void Restart() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(TagHolder.GAME_SCENE, LoadSceneMode.Single);
    }

    public void BackToMM() {
        Time.timeScale = 1f;
        endGame.endGame(false);
    }
}
