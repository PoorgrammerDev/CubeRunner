using UnityEngine;
public class GameValues : MonoBehaviour {

    //FORWARD SPEED---------
    private const float MAX_FORWARD_SPEED = 100f;

    [SerializeField]
    private float forwardSpeed = 10f;

    public float getForwardSpeed() {
        return forwardSpeed;
    }

    public void setForwardSpeed(float speed) {
        if (speed > 0 && speed < MAX_FORWARD_SPEED) {
            forwardSpeed = speed;
        }
        else {
            throw new System.Exception("Forward Speed is out of bounds.");
        }
    }

    //STRAFING SPEED---------

    [SerializeField]
    private float strafingSpeed = 10f;

    public float getStrafingSpeed() {
        return strafingSpeed;
    }

    public void setStrafingSpeed(float speed) {
        if (speed > 0) {
            strafingSpeed = speed;
        }
        else {
            throw new System.Exception("Strafing Speed cannot be negative or zero.");
        }
    }

    //ROW DISTANCE MULTIPLIER---------
    [SerializeField]
    private float rowDistMultLowerBound = 1.1f;
    
    [SerializeField]
    private float rowDistMultUpperBound = 2f;

    public float getRowDistMultLowerBound() {
        return rowDistMultLowerBound;
    }

    public void setRowDistMultLowerBound(float num) {
        if (num > 1) {
            if (num < rowDistMultUpperBound) {
                rowDistMultLowerBound = num;
            }
            else {
                throw new System.Exception("Lower bound cannot be more than upper bound.");
            }
        }
        else {
            throw new System.Exception("Lower bound cannot be less than 1.");
        }
    }

    public float getRowDistMultUpperBound() {
        return rowDistMultUpperBound;
    }

    public void setRowDistMultUpperBound(float num) {
        if (num > rowDistMultLowerBound) {
            rowDistMultLowerBound = num;
        }
        else {
            throw new System.Exception("Upper bound cannot be less than lower bound.");
        }
    }

    //SCORE---------
    [SerializeField]
    private float scoreIncreaseRate = 2f;

    private int score = 0;
    private int highScore;

    public float getScoreIncreaseRate() {
        return scoreIncreaseRate;
    }

    public int getScore() {
        return score;
    }

    public void setScore(int score) {
        this.score = score;
    }

    //GAP WIDTH MULTIPLIER---------
    private float widthMultiplier = 1f;

    public float getWidthMultiplier() {
        return widthMultiplier;
    }

    public void setWidthMultiplier(float num) {
        if (num > 0 && num < 3) {
            widthMultiplier = num;
        }
        else {
            throw new System.Exception("Width Multiplier is out of bounds.");
        }
    }

    //THEME---------

}