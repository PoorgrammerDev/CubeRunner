using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Managing Score incrementing and its display
/// </summary>
public class Score : MonoBehaviour
{
    [SerializeField] private GameValues gameValues;
    
    private Text scoreText;
    private WaitForSeconds wait;

    // Start is called before the first frame update
    void Start() {
        scoreText = GetComponent<Text>();
        wait = new WaitForSeconds(2f / gameValues.ScoreTickRate);

        StartCoroutine(ScoreTick());
    }

    IEnumerator ScoreTick() {
        while (gameValues.GameActive) {
            if (gameValues.PassedFirstObstacle) scoreText.text = (++gameValues.Score).ToString();
            yield return wait;
        }
    }
    
}