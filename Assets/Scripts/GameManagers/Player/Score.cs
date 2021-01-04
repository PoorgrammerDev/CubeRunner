using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
            scoreText.text = (++gameValues.Score).ToString();
            yield return new WaitForSeconds(2f / gameValues.ScoreTickRate);
        }
    }
    
}