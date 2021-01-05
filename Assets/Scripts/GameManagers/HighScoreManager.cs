using UnityEngine;

/// <summary>
/// Deals with High Score, including saving it to disk using PlayerPrefs
/// </summary>
public class HighScoreManager : MonoBehaviour {
    private int highScore;
    public int HighScore => highScore;

    void Awake() {
        highScore = PlayerPrefs.GetInt(TagHolder.PREF_HIGH_SCORE);
    }

    public bool ContestHighScore(int num) {
        if (num > highScore) {
            highScore = num;
            PlayerPrefs.SetInt(TagHolder.PREF_HIGH_SCORE, num);
            return true;
        }
        return false;
    }
}
