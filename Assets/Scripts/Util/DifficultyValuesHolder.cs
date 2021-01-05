using UnityEngine;

/// <summary>
/// Class used in Game Scene for setting game settings / values based on Difficulty.
/// </summary>
public class DifficultyValuesHolder : MonoBehaviour
{
    [SerializeField] private Difficulty difficulty;
    public Difficulty Difficulty => difficulty;
    

    //FORWARD SPEED---------
    [SerializeField] private float maxForwardSpeed;
    public float MaxForwardSpeed => maxForwardSpeed;

    [SerializeField] private float forwardSpeed;
    public float ForwardSpeed => forwardSpeed;

    //STRAFING SPEED---------
    [SerializeField] private float strafingSpeed;
    public float StrafingSpeed => strafingSpeed;

    //ROW DISTANCE MULTIPLIER---------
    [SerializeField] private float rowDistMultLowerBound;
    public float RowDistMultLowerBound => rowDistMultLowerBound;

    [SerializeField] private float rowDistMultUpperBound;
    public float RowDistMultUpperBound => rowDistMultUpperBound;

    //SCORE TICK RATE---------
    [SerializeField] private int scoreTickRate;
    public int ScoreTickRate => scoreTickRate;

    //GAP WIDTH MULTIPLIER---------
    [SerializeField] private float widthMultiplier;
    public float WidthMultiplier => widthMultiplier;

    //MINIMUM GAP WIDTH
    [SerializeField] private float minimumGapWidth;
    public float MinimumGapWidth => minimumGapWidth;
}
