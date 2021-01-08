using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles Ending the Game (Game stage side)
/// </summary>
public class EndGame : MonoBehaviour
{
    [SerializeField] private GameObject activePlayer;

    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private Beam beam;
    [SerializeField] private Treadmill treadmill;

    [SerializeField] private GameValues gameValues;

    [SerializeField] private SphereCollider explosionCollider;

    [SerializeField] private EndGameDataExport dataExport;

    [SerializeField] private GameObject[] invisibleWalls;

    [SerializeField] private uint divide = 4;

    private uint DEFAULT_DIVIDE = 4;

    private CubeGibs cubeGibs;
    private GameObject[] cubeParts;

    void Start() {
        if (!IsPowerOfTwo(divide)) {
            divide = DEFAULT_DIVIDE;
        }
        cubeGibs = GetComponent<CubeGibs>();
    }

    public void endGame() {
        //Data Export
        dataExport.FinalScore = gameValues.Score;
        dataExport.CubePartDivide = divide;
        dataExport.Difficulty = gameValues.Difficulty;

        //disable invisible borders
        foreach (GameObject invisibleWall in invisibleWalls) {
            invisibleWall.SetActive(false);
        }

        //Slow-mo effect
        Time.timeScale = 0.125f;

        //End slow-mo effect
        StartCoroutine(TimeResume(0.25f));

        //Disable movement, etc.
        StartCoroutine(DisableGame());
        
        //smashing cube
        cubeParts = cubeGibs.smashCube(activePlayer, playerPrefab, divide);
        explosionCollider.enabled = true;

        //Activate beam
        StartCoroutine(beam.ActivateBeam(activePlayer, cubeParts, 0.225f, activePlayer.transform.localScale.z / (float) divide, true));


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
