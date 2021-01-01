using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    [SerializeField]
    private GameObject gameOverScreen;

    [SerializeField]
    private GameObject mainMenuScreen;   

    [SerializeField]
    private GameObject beam; 

    [SerializeField]
    private GameObject spinningCube; 

    [SerializeField]
    private GameObject playerPropPrefab; 

    [SerializeField]
    private CubeGibs cubeGibs;
    private float spawnYPos = -2;
    private Quaternion quaternion = new Quaternion();

    private EndGameDataExport endGameDataExport;

    private GameObject[] cubeParts;

    private bool cubeConstructionBegan = false;

    void Awake() {
        endGameDataExport = FindObjectOfType<EndGameDataExport>();
        if (endGameDataExport != null) {
            gameOverScreen.SetActive(true);
            spinningCube.SetActive(false);
            beam.SetActive(true);
            StartCoroutine(CubeFormingAnimation());


            
        }
        else {
            mainMenuScreen.SetActive(true);
        }
    }

    IEnumerator CubeFormingAnimation() {
        //beam activate and move in
        beam.SetActive(true);
        beam.GetComponent<Animator>().Play(TagHolder.BEAM_ANIM_MENU_ENTER);

        //delay
        yield return new WaitForSeconds(1f);


        //spawn cube bits
        float partScale = 1f / (float) endGameDataExport.CubePartDivide;
        int parts = (int) endGameDataExport.CubePartDivide * 16;
        cubeParts = new GameObject[parts];
        for (int i = 0; i < parts; i++) {
            cubeParts[i] = cubeGibs.spawnCubeGib(playerPropPrefab, new Vector3(0, spawnYPos - (0.5f * i), cubeGibs.transform.position.z), quaternion, cubeGibs.transform, partScale);
            cubeParts[i].GetComponent<Rigidbody>().isKinematic = true;
        }

        //make cube bits flow into base
        Vector3 moveUp = new Vector3(0, 0.4f, 0);
        foreach (GameObject cubePart in cubeParts) {
            StartCoroutine(cubeRise(cubePart, moveUp));
            yield return null;
        }

        
    }

    IEnumerator cubeRise(GameObject cubePart, Vector3 riseVector) {
        Transform cubePartTransform = cubePart.transform;
        while (cubePartTransform.localPosition.y < -1.5f) {
            cubePartTransform.Translate(riseVector, Space.World);
            yield return null;
        }

        if (!cubeConstructionBegan) {
            //float mult = Mathf.Pow((1f / 9f), riseVector.y - 1); TODO cannot find a good formula for this, just going to set a value for now
            StartCoroutine(NewPlayerCubeForm(1, 1f / (2.5f * 16f * endGameDataExport.CubePartDivide)));
            cubeConstructionBegan = true;
        }

        cubePart.SetActive(false);
    }

    IEnumerator NewPlayerCubeForm (float scale, float speed) {
        bool hasAlreadyAppeared = false;
        Transform scTransform = spinningCube.transform;
        Vector3 scPosition = scTransform.localPosition; 
        Vector3 scScale = scTransform.localScale; 
        for (float i = 0; i < scale; i+= speed) {
            float yPos = -(scale / 2f) + (i / 2f);
            scPosition.y = yPos;
            scTransform.localPosition = scPosition;

            scScale.y = i;
            scTransform.localScale = scScale;

            if (!hasAlreadyAppeared) {
                spinningCube.SetActive(true);
                hasAlreadyAppeared = true;
            }
            yield return null;
        }

        scPosition.x = scPosition.y = scPosition.z = 0;
        scScale.x = scScale.y = scScale.z = scale;
        scTransform.localPosition = scPosition;
        scTransform.localScale = scScale;

        yield return new WaitForSeconds(0.5f);
        //beam exits
        beam.GetComponent<Animator>().Play(TagHolder.BEAM_ANIM_MENU_EXIT);
    }
}
