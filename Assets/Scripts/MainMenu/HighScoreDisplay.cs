using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class is meant to be placed directly on UI Text. Updates its value to match the correct High Score.
/// </summary>
public class HighScoreDisplay : MonoBehaviour
{
    [SerializeField] private HighScoreManager highScore;

    private Text numDisplay;

    // Start is called before the first frame update
    void Start() {
        numDisplay = GetComponent<Text>();
        UpdateScore();
    }

    public void UpdateScore() {
        numDisplay.text = highScore.HighScore.ToString();
    }
}
