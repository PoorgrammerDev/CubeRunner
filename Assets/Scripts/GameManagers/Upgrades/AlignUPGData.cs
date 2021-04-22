using UnityEngine;

[CreateAssetMenu(fileName = "New AlignUPGData", menuName = "Upgrade Stats/Align Upgrade Data")]
public class AlignUPGData : ScriptableObject {
    public AlignUPGEntry[] leftPath;
}

[System.Serializable]
public struct AlignUPGEntry {
    public float duration;
}