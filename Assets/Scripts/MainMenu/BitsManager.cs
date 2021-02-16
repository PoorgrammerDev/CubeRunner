using UnityEngine;

public class BitsManager : MonoBehaviour
{
    private int totalBits;

    public int TotalBits { get => totalBits; private set {
            totalBits = value;
            PlayerPrefs.SetInt(TagHolder.PREF_TOTAL_BITS, value);
        }
    }

    void Awake() {
        totalBits = PlayerPrefs.GetInt(TagHolder.PREF_TOTAL_BITS);
    }

    public void AddBits (int value) {
        TotalBits += value;
    }

    public bool SubtractBits (int value) {
        if (value >= totalBits) {
            TotalBits -= value;
            return true;
        }
        return false;
    }
}
