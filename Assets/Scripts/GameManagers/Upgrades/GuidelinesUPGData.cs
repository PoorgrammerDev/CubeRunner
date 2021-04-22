using UnityEngine;

[CreateAssetMenu(fileName = "New GuidelinesUPGData", menuName = "Upgrade Stats/Guidelines Upgrade Data")]
public class GuidelinesUPGData : ScriptableObject {
    public GuidelinesUPGEntry[] leftPath;
    public GuidelinesUPGEntry[] rightPath;
}

[System.Serializable]
public struct GuidelinesUPGEntry {
    public float duration;
    public float range;
}