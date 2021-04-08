using UnityEngine;

[CreateAssetMenu(fileName = "New Buy Menu Data", menuName = "Upgrade Menu/Buy Menu Data")]
public class BuyMenuData : ScriptableObject
{
    [SerializeField] private PowerUpType powerUpType;
    public PowerUpType PowerUpType => powerUpType;

    [SerializeField] private string pathName;
    public string PathName => pathName;

    [SerializeField] private int pathIndex;
    public int PathIndex => pathIndex;

    [SerializeField] private string description;
    public string Description => description;

    [SerializeField] private BuyMenuEntry[] data;

    public BuyMenuEntry GetDataEntry(int level) {
        if (level >= 0 && level < data.Length) {
            return data[level];
        }
        throw new System.Exception("Level is out of bounds!");
    }
}

[System.Serializable]
public struct BuyMenuEntry {
    public int cost;
    public string statName0;
    public string statChange0;
    
    public string statName1;
    public string statChange1;
}