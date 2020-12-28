using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    private int scoreBuffer = 0;

    [SerializeField]
    private int scoreBufferThreshold = 50;

    private Text scoreText;

    [SerializeField]
    private GameValues gameValues;

    // Start is called before the first frame update
    void Start() {
        scoreText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update() {
        if (++scoreBuffer > scoreBufferThreshold) {
            scoreBuffer = 0;
            gameValues.setScore(gameValues.getScore() + 1);
        }

        scoreText.text = "" + (int) gameValues.getScore();
    }
}
