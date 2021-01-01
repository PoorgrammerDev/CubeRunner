using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class EndGame : MonoBehaviour
{
    [SerializeField]
    private GameObject activePlayer;

    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private GameObject beam;

    [SerializeField]
    private GameValues gameValues;

    [SerializeField]
    private SphereCollider explosionCollider;

    [SerializeField]
    private EndGameDataExport dataExport;

    [SerializeField]
    private uint divide = 4;

    private uint DEFAULT_DIVIDE = 4;

    private CubeGibs cubeGibs;
    private GameObject[] cubeParts;
    private float CUBE_SUCK_CENTER_TIME = 0.0625f;
    private float CUBE_SUCK_RISE_TIME = 0.25f;
    private Quaternion quaternion = new Quaternion();

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

        //Slow-mo effect
        Time.timeScale = 0.125f;

        //End slow-mo effect
        StartCoroutine(TimeResume(0.125f));

        //Disable movement, etc.
        StartCoroutine(DisableGame());
        
        //smashing cube
        cubeParts = cubeGibs.smashCube(activePlayer, playerPrefab, divide);
        explosionCollider.enabled = true;

        //Begin beam sucking
        StartCoroutine(beamSuck(activePlayer.transform.localScale.z / (float) divide));
    }

    IEnumerator LoadNewScene() {
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
    }

    IEnumerator beamSuck(float partScale) {
        //intiial delay
        yield return new WaitForSeconds(1);

        beam.SetActive(true);

        //beam moves to match cube Z
        Vector3 beamPosition = beam.transform.position;
        beamPosition.z = activePlayer.transform.position.z;
        beam.transform.position = beamPosition;


        //beam moves down
        const float speedScale = 2.5f;
        Vector3 moveDown = new Vector3(0, -(1f / speedScale), 0);
        for (int i = 0; i < (beam.transform.position.y * speedScale); i++) {
            beam.transform.Translate(moveDown, Space.World);
            yield return null;
        }

        //cubes get sucked up
        for (int i = 0; i < cubeParts.Length; i++) {
            StartCoroutine(suckUpCube(cubeParts[i], partScale, i));
            yield return null;
        }

        GameObject finalCubePart = cubeParts[cubeParts.Length - 1];
        while (finalCubePart != null && finalCubePart.activeInHierarchy && finalCubePart.transform.position.y < 20) {
            yield return null;
        }

        //loads new scene
        StartCoroutine(LoadNewScene());
        yield break;
    }

    IEnumerator suckUpCube(GameObject cube, float partScale, int i) {
        //suck to center
        Transform cubeTransform = cube.transform;
        Vector3 currentPosition = cubeTransform.position;
        Vector3 targetPosition = new Vector3(0 + (Random.Range(-0.25f, 0.25f)), 1, activePlayer.transform.position.z + (Random.Range(-0.25f, 0.25f)));
        float step = 0f;
        while (step < 1) {
            step += Time.deltaTime / (CUBE_SUCK_CENTER_TIME + (0.025f * i));
            cubeTransform.position = Vector3.Lerp(currentPosition, targetPosition, step);
            yield return null;
        }

        //suck up
        currentPosition = targetPosition;
        targetPosition.y = 20;
        step = 0f;
        while (step < 1) {
            step += Time.deltaTime / CUBE_SUCK_RISE_TIME;
            cubeTransform.position = Vector3.Lerp(currentPosition, targetPosition, step);
            yield return null;
        }

        cube.SetActive(false);
        yield break;
    }

    bool IsPowerOfTwo(uint x) {
        return (x != 0) && ((x & (x - 1)) == 0);
    }
}
