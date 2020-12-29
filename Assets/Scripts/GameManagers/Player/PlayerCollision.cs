using UnityEngine;
using UnityEngine.UI;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField]
    private GameValues gameValues;

    [SerializeField]
    private GameObject gameOverScreen;
    
    [SerializeField]
    private GameObject runningScore;

    [SerializeField]
    private Text finalScores;

    private void OnTriggerEnter(Collider other) {
        GameObject otherObject = other.gameObject;
        if (otherObject != gameObject) {
            if (otherObject.CompareTag("Obstacle")) {
                if (otherObject.transform.position.x > 0) {
                    collideWithObstacle();
                }
            }
            else if (otherObject.CompareTag("PowerUp")) {

            }
        }
    }

    private void collideWithObstacle() {
        //TODO add power up exceptions here

        endGame();
    }

    public void endGame() {
        Time.timeScale = 0;

        gameOverScreen.SetActive(true);
        runningScore.SetActive(false);

        finalScores.text = gameValues.getScore() + "\n" + 0;
    }
}
