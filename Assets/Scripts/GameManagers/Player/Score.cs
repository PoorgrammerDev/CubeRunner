using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    private float scoreBuffer = 0;

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
        if (!gameValues.GameActive) return;
        
        scoreBuffer += Time.deltaTime * 200;

        if (scoreBuffer > scoreBufferThreshold) {
            scoreBuffer = 0;
            gameValues.Score++;
        }

        scoreText.text = ((int) gameValues.Score).ToString();
    }
}
