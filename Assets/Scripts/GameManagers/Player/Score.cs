using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Managing Score incrementing and its display
/// </summary>
public class Score : MonoBehaviour
{
    private Text scoreText;

    [SerializeField]
    private GameValues gameValues;

    // Start is called before the first frame update
    void Start() {
        scoreText = GetComponent<Text>();
        StartCoroutine(ScoreTick());
    }

    IEnumerator ScoreTick() {
        while (gameValues.GameActive) {
            if (gameValues.PassedFirstObstacle) scoreText.text = (++gameValues.Score).ToString();
            yield return new WaitForSeconds(2f / gameValues.ScoreTickRate);
        }
    }
    
}