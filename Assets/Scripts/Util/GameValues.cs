using UnityEngine;
public class GameValues : MonoBehaviour {

    //GAME ACTIVE-----------
    [SerializeField]
    private bool gameActive = true;
    public bool GameActive { get => gameActive; set => gameActive = value; }

    //DIFFICULTY------
    private Difficulty difficulty;
    public Difficulty Difficulty { get => difficulty; set => difficulty = value; }
    

    //FORWARD SPEED---------
    private const float maxForwardSpeed = 20f;
    public float MaxForwardSpeed => maxForwardSpeed;

    private float forwardSpeed = 7f;
    public float ForwardSpeed { get => forwardSpeed; set {
            if (value > 0 && value < MaxForwardSpeed) {
                ForwardSpeed = value;
            }
            else {
                throw new System.Exception("Forward Speed is out of bounds.");
            }
        }
    }

    //STRAFING SPEED---------
    private float strafingSpeed = 7f;
    public float StrafingSpeed { get => strafingSpeed;
        set {
            if (value > 0) {
                strafingSpeed = value;
            }
            else {
                throw new System.Exception("Strafing Speed cannot be negative or zero.");
            }
        }
    }

    

    //ROW DISTANCE MULTIPLIER---------
    private float rowDistMultLowerBound = 1.25f;
    public float RowDistMultLowerBound { get => rowDistMultLowerBound; set {
            if (value > 1) {
                if (value < RowDistMultUpperBound) {
                    RowDistMultLowerBound = value;
                }
                else {
                    throw new System.Exception("Lower bound cannot be more than upper bound.");
                }
            }
            else {
                throw new System.Exception("Lower bound cannot be less than 1.");
            }
        }
    }

    
    private float rowDistMultUpperBound = 1.75f;
    public float RowDistMultUpperBound { get => rowDistMultUpperBound; set {
            if (value > RowDistMultLowerBound) {
                RowDistMultLowerBound = value;
            }
            else {
                throw new System.Exception("Upper bound cannot be less than lower bound.");
            }
        }
    }

    //SCORE---------
    private int score = 0;
    public int Score {get => score; set => score = value;}

    //GAP WIDTH MULTIPLIER---------
    private float widthMultiplier = 1.25f;
    public float WidthMultiplier {get => widthMultiplier; set {
            if (value > 0 && value < 3) {
                widthMultiplier = value;
            }
            else {
                throw new System.Exception("Width Multiplier is out of bounds.");
            }
        }
    }

    //MINIMUM GAP WIDTH
    [SerializeField]
    private float minimumGapWidth = 1.5f;
    private float MinimumGapWidth {get => minimumGapWidth; set {
            if (value >= 1) {
                minimumGapWidth = value;
            }
            else {
                throw new System.Exception("Minimum Gap Width cannot be under one.");
            }
        }
    }    
}