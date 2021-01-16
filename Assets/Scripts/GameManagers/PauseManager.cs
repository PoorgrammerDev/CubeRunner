using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] Animator pauseMenu;
    [SerializeField] EndGame endGame;
    [SerializeField] GameObject player;

    public void Pause() {
        if (pauseMenu.gameObject.activeInHierarchy) return;

        Time.timeScale = 0f;
        pauseMenu.gameObject.SetActive(true);
    }

    public void Resume() {
        if (!pauseMenu.gameObject.activeInHierarchy) return;

        Time.timeScale = 1f;
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
