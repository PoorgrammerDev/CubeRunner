using UnityEngine;

public class EndGameDataExport : MonoBehaviour {
    [SerializeField]
    private int finalScore;
    public int FinalScore { get => finalScore; set => finalScore = value; }

[SerializeField]
    private uint cubePartDivide;
    public uint CubePartDivide { get => cubePartDivide; set => cubePartDivide = value; }

[SerializeField]
    private Difficulty difficulty;
    public Difficulty Difficulty { get => difficulty; set => difficulty = value; }
}
