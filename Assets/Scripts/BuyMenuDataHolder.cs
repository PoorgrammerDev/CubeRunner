using UnityEngine;

public class BuyMenuDataHolder : MonoBehaviour
{
    [SerializeField] private string description;
    public string Description => description;

    [SerializeField] private BuyMenuData[] data;

    public BuyMenuData GetBuyMenuData(int level) {
        if (level >= 0 && level < data.Length) {
            return data[level];
        }
        throw new System.Exception("Level is out of bounds!");
    }
}

[System.Serializable]
public struct BuyMenuData {
    public int cost;
    public string statName0;
    public string statName1;
    public string statChange0;
    public string statChange1;
}