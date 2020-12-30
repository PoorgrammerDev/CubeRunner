using UnityEngine;
using UnityEngine.UI;

public class HighScoreDisplay : MonoBehaviour
{
    [SerializeField]
    private HighScore highScore;

    private Text numDisplay;

    // Start is called before the first frame update
    void Start() {
        if (highScore == null) highScore = GetComponent<HighScore>();
        numDisplay = GetComponent<Text>();

        numDisplay.text = highScore.getHighScore(Difficulty.EASY).ToString(); //TODO change this to difficulty setting 
    }
}
