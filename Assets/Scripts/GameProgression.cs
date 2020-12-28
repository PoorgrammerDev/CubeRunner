using UnityEngine;

public class GameProgression : MonoBehaviour
{
    [SerializeField]
    private GameValues gameValues;

    [SerializeField]
    private CubeSpawner cubeSpawner;

    [SerializeField]
    private int progressionStep = 100;

    private int lastScoreRanAt = 0;

    // Update is called once per frame
    void Update() {
        int score = gameValues.getScore();
        if (IncreaseSpeed(score) | DecreaseGapWidth(score) | DecreaseGaps(score)) {
            lastScoreRanAt = score;
        }
    }

    bool IncreaseSpeed(int score) {
        if (score != 0 && score > lastScoreRanAt && score % progressionStep == 0) {
            gameValues.setForwardSpeed(gameValues.getForwardSpeed() + 1);
            gameValues.setStrafingSpeed(gameValues.getStrafingSpeed() + 1);

            return true;
        }
        return false;
    }

    bool DecreaseGapWidth(int score) {
        if (score != 0 && score > lastScoreRanAt && score % progressionStep == 0) {
            float widthMultiplier = gameValues.getWidthMultiplier();
            widthMultiplier *= 0.75f;
            if (widthMultiplier > 1) {
                gameValues.setWidthMultiplier(widthMultiplier);
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
