using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles Ending the Game (Game stage side)
/// </summary>
public class EndGame : MonoBehaviour
{
    [SerializeField] private GameObject activePlayer;

    [SerializeField] private Beam beam;
    [SerializeField] private Treadmill treadmill;

    [SerializeField] private GameValues gameValues;

    [SerializeField] private EndGameDataExport dataExport;

    [SerializeField] private GameObject[] invisibleWalls;

    [SerializeField] private GibManager playerGibManager;
    [SerializeField] private PlayerPowerUp playerPowerUp;
    [SerializeField] private TimeDilation timeDilation;
    [SerializeField] private Animator HUD;
    [SerializeField] private Animator NextPUPHUD;
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private AudioSource genericSFX;
    [SerializeField] private AudioClip deathSound;

    private GameObject[] cubeParts;
    public void endGame(bool effects) {
        if (playerPowerUp.GetActivePowerUp() == PowerUpType.TimeDilation) {
            timeDilation.ResetTDEffects(true);
        }

        //Data Export
        dataExport.FinalScore = gameValues.Score;
        dataExport.CubePartDivide = gameValues.Divide;
        dataExport.BitsCollected = gameValues.Bits;

        //remove hud
        HUD.SetTrigger(TagHolder.HUD_EXIT_TRIGGER);
        if (NextPUPHUD.gameObject.activeInHierarchy) NextPUPHUD.SetTrigger(TagHolder.HUD_EXIT_TRIGGER);
        
        //Disable movement, etc.
        StartCoroutine(DisableGame());

        //stop music
        musicManager.Pause();

        if (effects) {
            //sfx
            genericSFX.clip = deathSound;
            genericSFX.Play();

            //disable invisible borders
            foreach (GameObject invisibleWall in invisibleWalls) {
                invisibleWall.SetActive(false);
            }

            //Slow-mo effect
            Time.timeScale = 0.125f;

            //End slow-mo effect
            StartCoroutine(TimeResume(0.25f));

            //smashing cube
            cubeParts = playerGibManager.Activate(activePlayer.transform.position, activePlayer.transform.localScale, false, true);

            //Activate beam
            StartCoroutine(beam.ActivateBeam(activePlayer, cubeParts, 0.225f, activePlayer.transform.localScale.z / (float) gameValues.Divide, true));
        }
        else {
            StartCoroutine(LoadNewScene());
        }
    }

    public IEnumerator LoadNewScene() {
        Camera.main.GetComponent<AudioListener>().enabled = false;
        SceneManager.LoadScene(TagHolder.MAIN_MENU_SCENE, LoadSceneMode.Additive);
        Scene gameOver = SceneManager.GetSceneByName(TagHolder.MAIN_MENU_SCENE);
        SceneManager.MoveGameObjectToScene(dataExport.gameObject, gameOver);
        yield return null;

        foreach (GameObject obj in SceneManager.GetActiveScene().GetRootGameObjects()) {
            obj.SetActive(false);
        }
        SceneManager.UnloadSceneAsync(TagHolder.GAME_SCENE);
    }

    IEnumerator TimeResume(float delay) {
        yield return new WaitForSeconds(delay);
        Time.timeScale = 1f;
    }

    IEnumerator DisableGame() {
        yield return new WaitForSeconds(0.1f);
        gameValues.GameActive = false;
        treadmill.active = false;
    }

    bool IsPowerOfTwo(uint x) {
        return (x != 0) && ((x & (x - 1)) == 0);
    }
}
