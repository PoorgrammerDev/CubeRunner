using UnityEngine;

public class EndGameDataExport : MonoBehaviour {
    private int finalScore;
    private uint cubePartDivide;
    private Difficulty difficulty;
    public int FinalScore { get => finalScore; set => finalScore = value; }
    public uint CubePartDivide { get => cubePartDivide; set => cubePartDivide = value; }
    public Difficulty Difficulty { get => difficulty; set => difficulty = value; }
}
