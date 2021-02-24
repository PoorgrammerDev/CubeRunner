using UnityEngine;
using TMPro;

/// <summary>
/// Managing Score incrementing and its display
/// </summary>
public class Score : MonoBehaviour
{
    [SerializeField] private GameValues gameValues;
    [SerializeField] private GameProgression gameProgression;
    
    private float realScore;
    private TextMeshProUGUI scoreText;

    // Start is called before the first frame update
    void Start() {
        scoreText = GetComponent<TextMeshProUGUI>();
    }

    void Update() {
        if (!gameValues.GameActive || !gameValues.PassedFirstObstacle) return;

        realScore += gameValues.ForwardSpeed * Time.deltaTime;
        gameValues.Score = (int) realScore;

        gameProgression.CheckForProgression();
        scoreText.text = gameValues.Score.ToString();
    }
}