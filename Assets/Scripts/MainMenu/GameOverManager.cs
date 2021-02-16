using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages the Game Over screen on the Main Menu, including all the effects on there.
/// </summary>
public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverScreen;

    [SerializeField] private GameObject mainMenuScreen;

    [SerializeField] private TextMeshProUGUI scoreNumber;
    [SerializeField] private Animator scoreAnim;
    [SerializeField] private TextMeshProUGUI bitsNumber;

    [SerializeField] private GameObject beam; 

    [SerializeField] private GameObject spinningCube; 

    [SerializeField] private GameObject playerPropPrefab; 

    [SerializeField] private GameObject[] delayedEnable;

    [SerializeField] private CubeGibsUtil cubeGibs;

    [SerializeField] private HighScoreManager highScoreManager;
    [SerializeField] private BitsManager bitsManager;

    private EndGameDataExport endGameDataExport;

    private GameObject[] cubeParts;

    private float spawnYPos = -2;

    void Awake() {
        endGameDataExport = FindObjectOfType<EndGameDataExport>();
        if (endGameDataExport != null) {            
            gameOverScreen.SetActive(true);

            //display score
            scoreNumber.text = endGameDataExport.FinalScore + "m";

            //check high score and play effect if new score is higher
            if (highScoreManager.ContestHighScore(endGameDataExport.FinalScore)) {
                scoreAnim.SetTrigger(TagHolder.ANIM_HIGH_SCORE_SUCCESS);
            }

            //display bits
            bitsNumber.text = endGameDataExport.BitsCollected.ToString();

            //increment bits
            bitsManager.AddBits(endGameDataExport.BitsCollected);

            //dont call anims and immediately enable delayed objects
            if (PlayerPrefs.GetInt(TagHolder.PREF_SKIP_ANIM) == 1) {
                //enable the delayed objects
                foreach (GameObject delayed in delayedEnable) {
                    delayed.SetActive(true);
                }
            }

            //start animations
            else {
                spinningCube.SetActive(false);
                StartCoroutine(CubeFormingAnimation());
            }

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
        int parts = (int) (endGameDataExport.CubePartDivide * endGameDataExport.CubePartDivide * endGameDataExport.CubePartDivide);
        cubeParts = new GameObject[parts];
        for (int i = 0; i < parts; i++) {
            cubeParts[i] = cubeGibs.DeployGib(playerPropPrefab, new Vector3(0, spawnYPos - (2f * i), cubeGibs.transform.position.z), cubeGibs.transform, partScale, partScale, partScale, true);
            cubeParts[i].GetComponent<Rigidbody>().isKinematic = true;
        }

        //cube begins forming
        StartCoroutine(NewPlayerCubeForm(2));

        //make cube bits flow into base
        for (int i = 0; i < cubeParts.Length; i++) {
            StartCoroutine(CubeRise(cubeParts[i], cubeParts.Length / 4, i));
        }
    }

    IEnumerator CubeRise(GameObject cubePart, float speed, int i) {
        Transform cubePartTransform = cubePart.transform;
        Vector3 position = cubePartTransform.localPosition;
        float origY = position.y;

        float t = 0f;
        while (t <= 1) {
            t += (speed * Time.deltaTime) / (0.1f * i);
            
            //move position up
            position.y = Mathf.Lerp(origY, -0.75f, t);
            cubePartTransform.localPosition = position;

            yield return null;
        }

        Destroy(cubePart);
    }

    IEnumerator NewPlayerCubeForm (float speed) {
        Transform scTransform = spinningCube.transform;
        Vector3 position = scTransform.localPosition; 
        Vector3 scale = scTransform.localScale; 
        
        //cube appears
        spinningCube.SetActive(true);
        
        float t = 0f;
        while (t <= 1) {
            t += speed * Time.deltaTime;
            float change = Mathf.Lerp(0, 1, t);

            //change position, starts at -0.5 and climbs up to 0
            position.y = -0.5f + (change / 2f);
            scTransform.localPosition = position;

            //change scale, starts at 0 and climbs up to 1
            scale.y = change;
            scTransform.localScale = scale;

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        
        //enable the delayed objects
        foreach (GameObject delayed in delayedEnable) {
            delayed.SetActive(true);
        }
        //beam exit
        beam.GetComponent<Animator>().Play(TagHolder.BEAM_ANIM_MENU_EXIT);
    }

    public void BackToMainMenu() {
        Destroy(endGameDataExport.gameObject);
        gameOverScreen.SetActive(false);
        mainMenuScreen.SetActive(true);
    }
}
