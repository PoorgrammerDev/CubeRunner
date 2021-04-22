using UnityEngine;

[CreateAssetMenu(fileName = "New BlasterUPGData", menuName = "Upgrade Stats/Blaster Upgrade Data")]
public class BlasterUPGData : ScriptableObject {
    public BlasterUPGEntry[] leftPath;
    public BlasterUPGEntry[] rightPath;
}

[System.Serializable]
public struct BlasterUPGEntry {
    public float cooldown;
    public int ammo;
    public float range;
    public int piercing;
    
}