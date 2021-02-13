﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private Animator pauseMenu;
    [SerializeField] private Animator GameHUD;
    [SerializeField] private EndGame endGame;
    [SerializeField] private GameObject player;
    [SerializeField] private MusicManager musicManager;

    [SerializeField] private Button pauseButton;

    public bool paused = false;

    private float currentTimeScale = -1;

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

        paused = true;
        currentTimeScale = Time.timeScale;
        GameHUD.ResetTrigger(TagHolder.HUD_ENTER_TRIGGER);
        GameHUD.SetTrigger(TagHolder.HUD_EXIT_TRIGGER);
        Time.timeScale = 0f;
        musicManager.Pause();
        pauseMenu.gameObject.SetActive(true);
    }

    public void Resume() {
        if (!pauseMenu.gameObject.activeInHierarchy) return;

        paused = false;
        GameHUD.ResetTrigger(TagHolder.HUD_EXIT_TRIGGER);
        GameHUD.SetTrigger(TagHolder.HUD_ENTER_TRIGGER);
        
        Time.timeScale = (currentTimeScale != -1) ? currentTimeScale : 1;
        currentTimeScale = -1;
        musicManager.Resume();
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
