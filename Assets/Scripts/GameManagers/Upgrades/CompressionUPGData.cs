using UnityEngine;

[CreateAssetMenu(fileName = "New CompressionUPGData", menuName = "Upgrade Stats/Compression Upgrade Data")]
public class CompressionUPGData : ScriptableObject {
    public CompressionUPGEntry[] leftPath;
    public CompressionUPGEntry[] rightPath;
}

[System.Serializable]
public struct CompressionUPGEntry {
    public float duration;
    public float shrinkRate;
}