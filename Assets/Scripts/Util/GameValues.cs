using UnityEngine;

/// <summary>
/// Class used to hold values integral to game.
/// </summary>
public class GameValues : MonoBehaviour {

    [SerializeField] private DifficultyValuesHolder[] difficultyValuesHolders;
    void Awake() {
        difficulty = (Difficulty) PlayerPrefs.GetInt(TagHolder.PREF_DIFFICULTY);

        //set values
        DifficultyValuesHolder difficultyValuesHolder = difficultyValuesHolders[(int) difficulty];
        maxForwardSpeed = difficultyValuesHolder.MaxForwardSpeed;
        forwardSpeed = difficultyValuesHolder.ForwardSpeed;
        strafingSpeed = difficultyValuesHolder.StrafingSpeed;
        rowDistMultLowerBound = difficultyValuesHolder.RowDistMultLowerBound;
        rowDistMultUpperBound = difficultyValuesHolder.RowDistMultUpperBound;
        scoreTickRate = difficultyValuesHolder.ScoreTickRate;
        gapIncreaseChance = difficultyValuesHolder.GapIncreaseChance;
    }

    //GAME ACTIVE-----------

    [SerializeField]
    private bool gameActive = false;
    public bool GameActive { get => gameActive; set => gameActive = value;}

    //PASSED FIRST OBSTACLE-----------
    private bool passedFirstObstacle = false;
    public bool PassedFirstObstacle { get => passedFirstObstacle; set => passedFirstObstacle = value; }

    //DIFFICULTY------
    private Difficulty difficulty;
    public Difficulty Difficulty => difficulty;
    

    //FORWARD SPEED---------
    private float maxForwardSpeed;
    public float MaxForwardSpeed => maxForwardSpeed;

    private float forwardSpeed;
    public float ForwardSpeed { get => forwardSpeed; set {
            if (value > 0 && value < maxForwardSpeed) {
                forwardSpeed = value;
            }
            else {
                throw new System.Exception("Forward Speed is out of bounds.");
            }
        }
    }

    //STRAFING SPEED---------
    private float strafingSpeed;
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
    private float rowDistMultLowerBound;
    public float RowDistMultLowerBound { get => rowDistMultLowerBound; set {
            if (value > 1) {
                if (value < rowDistMultUpperBound) {
                    rowDistMultLowerBound = value;
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

    
    private float rowDistMultUpperBound;
    public float RowDistMultUpperBound { get => rowDistMultUpperBound; set {
            if (value > rowDistMultLowerBound) {
                rowDistMultLowerBound = value;
            }
            else {
                throw new System.Exception("Upper bound cannot be less than lower bound.");
            }
        }
    }

    //SCORE---------
    private int score = 0;
    public int Score {get => score; set => score = value;}

    //SCORE TICK RATE---------
    private int scoreTickRate;
    public int ScoreTickRate {get => scoreTickRate; set => scoreTickRate = value;}

    //GAP INCREASE CHANCE---------
    private float gapIncreaseChance;
    public float GapIncreaseChance { get => gapIncreaseChance; set => gapIncreaseChance = value; }
    
}