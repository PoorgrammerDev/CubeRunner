using UnityEngine;

/// <summary>
/// Handles intra-Game progression (stage getting faster as it goes)
/// </summary>
public class GameProgression : MonoBehaviour
{
    [SerializeField] private GameValues gameValues;
    [SerializeField] private int baseProgStep;

    private int totalSteps;
    private int nextScore;
    
    private float startingFwdSpeed;
    private float startingStrfSpeed;

    void Start() {
        //stores speed at the start
        startingFwdSpeed = gameValues.ForwardSpeed;
        startingStrfSpeed = gameValues.StrafingSpeed;

        RecalculateValues();
    }

    public bool CheckForProgression() {
        if (gameValues.Score >= nextScore) {
            //speed limiter
            if (startingFwdSpeed + (totalSteps + 1) <= gameValues.MaxForwardSpeed) {
                totalSteps++;
                RecalculateValues();
                return true;
            }
        }
        return false;
    }

    void RecalculateValues() {
        //update speed values
        gameValues.ForwardSpeed = startingFwdSpeed + totalSteps;
        gameValues.StrafingSpeed = startingStrfSpeed + totalSteps;

        //update next score with new prog step (based on speed)
        nextScore = gameValues.Score + ((int) (baseProgStep * (gameValues.ForwardSpeed / startingFwdSpeed)));
    }

}
