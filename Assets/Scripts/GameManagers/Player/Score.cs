using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

/// <summary>
/// Managing Score incrementing and its display
/// </summary>
public class Score : MonoBehaviour
{
    [SerializeField] private GameValues gameValues;
    [SerializeField] private GameProgression gameProgression;
    
    private TextMeshProUGUI scoreText;
    private WaitForSeconds wait;

    // Start is called before the first frame update
    void Start() {
        scoreText = GetComponent<TextMeshProUGUI>();
        wait = new WaitForSeconds(5f / gameValues.ForwardSpeed);

        StartCoroutine(ScoreTick());
    }

    IEnumerator ScoreTick() {
        while (gameValues.GameActive) {
            if (gameValues.PassedFirstObstacle) {
                scoreText.text = (++gameValues.Score).ToString();
                gameProgression.CheckForProgression();
            }
            yield return wait;
        }
    }
    
}