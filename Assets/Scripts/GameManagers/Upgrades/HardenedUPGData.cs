using UnityEngine;

[CreateAssetMenu(fileName = "New HardenedUPGData", menuName = "Upgrade Stats/Hardened Upgrade Data")]
public class HardenedUPGData : ScriptableObject {
    public HardenedUPGEntry[] leftPath;
    public HardenedUPGEntry[] rightPath;
}

[System.Serializable]
public struct HardenedUPGEntry {
    public int duration;
    public int health;
    public int impactCost;
}