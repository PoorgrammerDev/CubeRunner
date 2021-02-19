using UnityEngine;

/// <summary>
/// Handles intra-Game progression (stage getting faster as it goes, etc.)
/// </summary>
public class GameProgression : MonoBehaviour
{
    [SerializeField] private GameValues gameValues;

    [SerializeField] private CubeSpawner cubeSpawner;

    [SerializeField] private int progressionStep;

    private int lastScoreRanAt = 0;

    public void CheckForProgression() {
        int score = gameValues.Score;
        if (IncreaseSpeed(score)) {
            lastScoreRanAt = score;
        }
    }

    bool IncreaseSpeed(int score) {
        if (score != 0 && score > lastScoreRanAt && score % progressionStep == 0) {
            float forwardSpeed = gameValues.ForwardSpeed;
            if (++forwardSpeed < gameValues.MaxForwardSpeed) {
                gameValues.ForwardSpeed = forwardSpeed;
                gameValues.StrafingSpeed = (gameValues.StrafingSpeed + 1);
            }
            return true;
        }
        return false;
    }

}
