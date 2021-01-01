using UnityEngine;
using System;

public class HighScoreManager : MonoBehaviour {

    private const int difficulties = 5;
    private int[] highScores = new int[difficulties];

    private string[] highScoreKeys = {
        TagHolder.PREF_HIGH_SCORE_CUSTOM,
        TagHolder.PREF_HIGH_SCORE_EASY,
        TagHolder.PREF_HIGH_SCORE_MEDIUM,
        TagHolder.PREF_HIGH_SCORE_HARD,
        TagHolder.PREF_HIGH_SCORE_IMPOSSIBLE
    };

    // Start is called before the first frame update
    void Start() {
        for (int i = 0; i < difficulties; i++) {
            highScores[i] = PlayerPrefs.GetInt(highScoreKeys[i]);
        }
    }
    public int getHighScore(Difficulty difficulty) {
        return highScores[(int) difficulty];
    }

    public void ContestHighScore(int num, Difficulty difficulty) {
        if (num > highScores[(int) difficulty]) {
            highScores[(int) difficulty] = num;
            PlayerPrefs.SetInt(highScoreKeys[(int) difficulty], num);
        }
    }
}
