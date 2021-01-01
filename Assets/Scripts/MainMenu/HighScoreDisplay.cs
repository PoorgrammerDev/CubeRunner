using UnityEngine;
using UnityEngine.UI;

//THIS SCRIPT IS MEANT TO BE PLACED DIRECTLY ON TEXT OBJECT
public class HighScoreDisplay : MonoBehaviour
{
    [SerializeField]
    private HighScoreManager highScore;

    private Text numDisplay;

    // Start is called before the first frame update
    void Start() {
        numDisplay = GetComponent<Text>();

        numDisplay.text = highScore.getHighScore(Difficulty.CUSTOM).ToString(); //TODO change this to difficulty setting 
    }
}
