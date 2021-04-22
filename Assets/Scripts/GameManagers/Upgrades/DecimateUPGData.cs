using UnityEngine;

[CreateAssetMenu(fileName = "New DecimateUPGData", menuName = "Upgrade Stats/Decimate Upgrade Data")]
public class DecimateUPGData : ScriptableObject {
    public DecimateUPGEntry[] leftPath;
}

[System.Serializable]
public struct DecimateUPGEntry {
    public int rows;
}