using UnityEngine;

/// <summary>
/// Class used in Game Scene for setting game settings / values based on Difficulty.
/// </summary>
public class DifficultyValuesHolder : MonoBehaviour
{
    [SerializeField] private Difficulty difficulty;
    public Difficulty Difficulty => difficulty;
    
    [Header("Speed")]
    //FORWARD SPEED---------
    [SerializeField] private float maxForwardSpeed;
    public float MaxForwardSpeed => maxForwardSpeed;

    [SerializeField] private float forwardSpeed;
    public float ForwardSpeed => forwardSpeed;

    //STRAFING SPEED---------
    [SerializeField] private float strafingSpeed;
    public float StrafingSpeed => strafingSpeed;

    [Header("Row Distance")]
    //ROW DISTANCE MULTIPLIER---------
    [SerializeField] private float rowDistMultLowerBound;
    public float RowDistMultLowerBound => rowDistMultLowerBound;

    [SerializeField] private float rowDistMultUpperBound;
    public float RowDistMultUpperBound => rowDistMultUpperBound;

    [Header("Gaps")]
    //GAP INCREASE CHANCE---------
    [SerializeField] private float gapIncreaseChance;
    public float GapIncreaseChance { get => gapIncreaseChance; set => gapIncreaseChance = value; }

    //GAP WIDTH SCALE
    [SerializeField] private float widthScale;
    public float WidthScale { get => widthScale; set => widthScale = value; }

    [Header("Other")]
    //SCORE TICK RATE---------
    [SerializeField] private int scoreTickRate;
    public int ScoreTickRate => scoreTickRate;

    //POWER UP SPAWN CHANCE
    [SerializeField] private float powerUpSpawnChance;
    public float PowerUpSpawnChance { get => powerUpSpawnChance; set => powerUpSpawnChance = value; }
    
    //MUSIC
    [SerializeField] private AudioClip[] songs;
    public AudioClip[] Songs { get => songs; set => songs = value; }
    
}
