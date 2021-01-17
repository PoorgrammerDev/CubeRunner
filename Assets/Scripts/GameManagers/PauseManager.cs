using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private Animator pauseMenu;
    [SerializeField] private EndGame endGame;
    [SerializeField] private GameObject player;
    [SerializeField] private MusicManager musicManager;

    public void Pause() {
        if (pauseMenu.gameObject.activeInHierarchy) return;

        Time.timeScale = 0f;
        musicManager.Pause();
        pauseMenu.gameObject.SetActive(true);
    }

    public void Resume() {
        if (!pauseMenu.gameObject.activeInHierarchy) return;

        Time.timeScale = 1f;
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
