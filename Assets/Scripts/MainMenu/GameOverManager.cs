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


    private EndGameDataExport endGameDataExport;

    void Awake() {
        endGameDataExport = FindObjectOfType<EndGameDataExport>();
        if (endGameDataExport != null) {
            gameOverScreen.SetActive(true);
            cubeFormingAnimation();


            
        }
        else {
            mainMenuScreen.SetActive(true);
        }
    }

    void cubeFormingAnimation() {
        beam.SetActive(true);
        StartCoroutine(beamExtend());
    }

    IEnumerator beamExtend() {
        const float TARGET_Y = -5.6f;
        const float MAGNITUDE = 0.1f;

        Transform beamTransform = beam.transform;
        Vector3 beamPosition = beamTransform.localPosition;

        for (float i = beamPosition.y; i < (TARGET_Y - MAGNITUDE); i += MAGNITUDE) {
            beamPosition.y += MAGNITUDE;
            beamTransform.localPosition = beamPosition;
            yield return null;
        }
    }
}
