using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ALWAYS attach this script directly onto BEAM
public class Beam : MonoBehaviour
{
    [SerializeField]
    private EndGame endGame;

    private Transform ZAligner;
    private Animator animator;
    private MeshRenderer renderer;

    private const float CUBE_SUCK_CENTER_TIME = 0.0625f;
    private const float CUBE_SUCK_RISE_TIME = 0.25f;

    void Start() {
        animator = GetComponent<Animator>();
        renderer = GetComponent<MeshRenderer>();
        ZAligner = transform.parent;
    }

    public IEnumerator ActivateBeam(GameObject activePlayer, GameObject[] cubeParts, float initialDelay, float partScale, bool autoSuck) {
        //intiial delay
        yield return new WaitForSeconds(initialDelay);

        renderer.enabled = true;

        //beam's z-aligner moves to match cube Z
        Vector3 beamPosition = ZAligner.position;
        beamPosition.z = activePlayer.transform.position.z;
        ZAligner.position = beamPosition;

        //beam moves down
        animator.Play(TagHolder.BEAM_ANIM_ENTER);

        //begins sucking up cubes
        if (autoSuck) {
            StartCoroutine(BeamSuck(cubeParts, partScale));
        }
    }

    IEnumerator BeamSuck(GameObject[] cubeParts, float partScale) {
        return BeamSuck(cubeParts, CUBE_SUCK_CENTER_TIME, CUBE_SUCK_RISE_TIME, partScale);
    }

    IEnumerator BeamSuck(GameObject[] cubeParts, float timeToCenter, float timetoTop, float partScale) {
        //cubes get sucked up
        for (int i = 0; i < cubeParts.Length; i++) {
            StartCoroutine(SuckUpCube(cubeParts[i], timeToCenter, timetoTop, partScale, i));
            yield return null;
        }

        GameObject finalCubePart = cubeParts[cubeParts.Length - 1];
        while (finalCubePart != null && finalCubePart.activeInHierarchy && finalCubePart.transform.position.y < 20) {
            yield return null;
        }

        StartCoroutine(endGame.LoadNewScene());
    }

    IEnumerator SuckUpCube(GameObject cube, float timeToCenter, float timetoTop, float partScale, int i) {
        //suck to center
        Transform cubeTransform = cube.transform;
        Vector3 currentPosition = cubeTransform.position;
        Vector3 targetPosition = new Vector3(0 + (Random.Range(-0.25f, 0.25f)), 1, transform.position.z + (Random.Range(-0.25f, 0.25f)));
        float step = 0f;
        while (step < 1) {
            step += Time.deltaTime / (timeToCenter + (0.025f * i));
            cubeTransform.position = Vector3.Lerp(currentPosition, targetPosition, step);
            yield return null;
        }

        //suck up
        currentPosition = targetPosition;
        targetPosition.y = 20;
        step = 0f;
        while (step < 1) {
            step += Time.deltaTime / timetoTop;
            cubeTransform.position = Vector3.Lerp(currentPosition, targetPosition, step);
            yield return null;
        }

        cube.SetActive(false);
    }

}
