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
    [SerializeField] private AsyncSceneLoader sceneLoader;

    [SerializeField] private GameObject[] invisibleWalls;

    [SerializeField] private GibManager playerGibManager;
    [SerializeField] private GibManager obstacleGibManager;
    [SerializeField] private PlayerPowerUp playerPowerUp;
    [SerializeField] private TimeDilation timeDilation;
    [SerializeField] private Animator HUD;
    [SerializeField] private Animator NextPUPHUD;
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private AudioSource genericSFX;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private GameObject skipButton;

    private GameObject[] cubeParts;
    private bool calledSceneLoad = false;
    public void endGame(bool effects) {
        if (playerPowerUp.GetActivePowerUp() == PowerUpType.TimeDilation) {
            timeDilation.ResetTDEffects(true);
        }

        //Remove PUPs and force stop sfx
        playerPowerUp.RemovePowerUp();
        playerPowerUp.StopSound();

        //Data Export
        dataExport.FinalScore = gameValues.Score;
        dataExport.CubePartDivide = gameValues.Divide;
        dataExport.BitsCollected = gameValues.Bits;
        DontDestroyOnLoad(dataExport.gameObject);

        //remove hud
        HUD.SetTrigger(TagHolder.HUD_EXIT_TRIGGER);
        if (NextPUPHUD.gameObject.activeInHierarchy) NextPUPHUD.SetTrigger(TagHolder.HUD_EXIT_TRIGGER);
        
        //Disable movement, etc.
        StartCoroutine(DisableGame());

        //stop music
        musicManager.Pause();

        if (effects) {
            Transform playerTransform = activePlayer.transform;

            //sfx
            genericSFX.clip = deathSound;
            genericSFX.Play();

            //disable invisible borders
            foreach (GameObject invisibleWall in invisibleWalls) {
                invisibleWall.SetActive(false);
            }

            //Slow-mo effect
            Time.timeScale = 0.125f;
            StartCoroutine(TimeResume(0.25f));

            //Hit Obstacle Gibbing
            Collider[] allObstacles = Physics.OverlapBox(playerTransform.position, playerTransform.localScale / 2f, Quaternion.identity, obstacleLayer, QueryTriggerInteraction.Collide);
            foreach (Collider obstacle in allObstacles) {
                obstacle.gameObject.SetActive(false);
                
                GameObject[] gibs = obstacleGibManager.Activate(obstacle.transform.position, obstacle.transform.localScale, false, true);
                foreach (GameObject gib in gibs) {
                    gib.GetComponent<Rigidbody>().AddForceAtPosition(Vector3.right * (gameValues.ForwardSpeed / 10.0f), playerTransform.position, ForceMode.Impulse);
                }
            }
            
            //Skip button appears
            skipButton.SetActive(true);

            //Player Gibbing
            cubeParts = playerGibManager.Activate(playerTransform.position, playerTransform.localScale, false, true);

            //Activate beam
            StartCoroutine(beam.ActivateBeam(activePlayer, cubeParts, 0.225f, playerTransform.localScale.z / (float) gameValues.Divide, true));
        }

        //Begin loading new scene
        sceneLoader.BeginAsyncProcess(false);
    }

    public void SkipButton() {
        if (Time.timeScale != 1) {
            Time.timeScale = 1;
            StopCoroutine(TimeResume(0));
        }

        sceneLoader.ActivateScene();
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
