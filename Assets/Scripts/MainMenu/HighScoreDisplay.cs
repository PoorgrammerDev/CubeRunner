using UnityEngine;
using TMPro;

/// <summary>
/// Class is meant to be placed directly on UI Text. Updates its value to match the current High Score.
/// </summary>
public class HighScoreDisplay : MonoBehaviour
{
    [SerializeField] private SaveManager saveManager;

    private TextMeshProUGUI numDisplay;

    // Start is called before the first frame update
    void Start() {
        numDisplay = GetComponent<TextMeshProUGUI>();
        UpdateScore();
    }

    public void UpdateScore() {
        numDisplay.text = saveManager.HighScore + "m";
    }
}
