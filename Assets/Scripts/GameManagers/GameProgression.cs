using UnityEngine;

public class GameProgression : MonoBehaviour
{
    [SerializeField]
    private GameValues gameValues;

    [SerializeField]
    private CubeSpawner cubeSpawner;

    [SerializeField]
    bool increaseSpeed;

    [SerializeField]
    bool decreaseGapWidth;
    
    [SerializeField]
    bool decreaseGaps;

    [SerializeField]
    private int progressionStep = 100;

    private int lastScoreRanAt = 0;

    // Update is called once per frame
    void Update() {
        int score = gameValues.Score;
        if ((increaseSpeed && IncreaseSpeed(score)) | (decreaseGapWidth && DecreaseGapWidth(score)) | (decreaseGaps && DecreaseGaps(score))) {
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

    bool DecreaseGapWidth(int score) {
        if (score != 0 && score > lastScoreRanAt && score % progressionStep == 0) {
            float widthMultiplier = gameValues.WidthMultiplier;
            widthMultiplier *= 0.9f;
            if (widthMultiplier > 1) {
                gameValues.WidthMultiplier = widthMultiplier;
            }
            return true;
        }     
        return false;   
    }
    
    bool DecreaseGaps(int score) {
        if (score != 0 && score > lastScoreRanAt && score % (progressionStep * 2) == 0) {
            int maxGaps = cubeSpawner.getMaxGaps();
            if (maxGaps > 1) {
                cubeSpawner.setMaxGaps(maxGaps - 1);
            }
            return true;
        }  
        return false;      
    }
}
