using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EndGame : MonoBehaviour
{
    [SerializeField]
    private GameObject activePlayer;

    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private Volume postProcessing;

    [SerializeField]
    private GameObject beam;

    [SerializeField]
    private GameValues gameValues;

    [SerializeField]
    private SphereCollider explosionCollider;

    [SerializeField]
    private uint divide = 4;

    private uint DEFAULT_DIVIDE = 4;

    private GameObject[] cubeParts;
    private float CUBE_SUCK_CENTER_TIME = 0.0625f;
    private float CUBE_SUCK_RISE_TIME = 0.25f;
    private Quaternion quaternion = new Quaternion();

    void Start() {
        if (!IsPowerOfTwo(divide)) {
            divide = DEFAULT_DIVIDE;
        }
    }

    public void endGame() {
        Time.timeScale = 0.125f;

        StartCoroutine(TimeResume(0.125f));
        StartCoroutine(DisableGame());
        smashCube();
    }

    IEnumerator TimeResume(float delay) {
        yield return new WaitForSeconds(delay);
        Time.timeScale = 1f;
    }

    IEnumerator DisableGame() {
        yield return new WaitForSeconds(0.1f);
        gameValues.GameActive = false;
    }

    public void smashCube() {
        Vector3 activePlayerPos = activePlayer.transform.localPosition;
        float activePlayerScale = activePlayer.transform.localScale.z;
        activePlayer.GetComponent<BoxCollider>().enabled = false;
        activePlayer.GetComponent<CharacterController>().enabled = false;
        activePlayer.GetComponent<MeshRenderer>().enabled = false;

        float partScale = activePlayerScale / (float) divide;
        uint parts = divide * 16;
        cubeParts = new GameObject[parts];

        int count = 0;
        for (int x = 0; x < divide; x++) {
            for (int y = 0; y < divide; y++) {
                for (int z = 0; z < divide; z++) {
                    float xPos = activePlayerPos.x + ((activePlayerScale / 2f) - (partScale / 2f)) - (x * partScale);
                    float yPos = activePlayerPos.y + ((activePlayerScale / 2f) - (partScale / 2f)) - (y * partScale);
                    float zPos = activePlayerPos.z + ((activePlayerScale / 2f) - (partScale / 2f)) - (z * partScale);

                    GameObject part = Instantiate(playerPrefab, new Vector3(xPos, yPos, zPos), quaternion);
                    Vector3 partScaleVector = part.transform.localScale;
                    partScaleVector.x = partScaleVector.y = partScaleVector.z = partScale;
                    part.transform.localScale = partScaleVector;

                    cubeParts[count] = part;
                    count++;
                }
            }
        }

        explosionCollider.enabled = true;
        StartCoroutine(beamSuck(partScale));
    }

    IEnumerator beamSuck(float partScale) {
        //intiial delay
        yield return new WaitForSeconds(1);

        beam.SetActive(true);

        Transform beamTransform = beam.transform;
        //beam moves to match cube Z
        Vector3 beamPosition = transform.position;
        beamPosition.z = activePlayer.transform.position.z;
        beamTransform.position = beamPosition;


        //beam moves down
        Vector3 moveDown = new Vector3(0, -0.05f, 0);
        for (int i = 0; i < ((int) transform.position.x - 5); i++) {
            beamTransform.Translate(moveDown, Space.World);
            yield return new WaitForSeconds(1);
        }

        //cubes get sucked up
        for (int i = 0; i < cubeParts.Length; i++) {
            StartCoroutine(suckUpCube(cubeParts[i], partScale, i));
            yield return null;
        }

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
