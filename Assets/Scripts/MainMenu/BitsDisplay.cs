using UnityEngine;
using TMPro;

/// <summary>
/// Class is meant to be placed directly on UI Text. Updates its value to match the current Total Bits.
/// </summary>
public class BitsDisplay : MonoBehaviour
{
    [SerializeField] private BitsManager bitsManager;

    private TextMeshProUGUI numDisplay;

    // Start is called before the first frame update
    void Start() {
        numDisplay = GetComponent<TextMeshProUGUI>();
        UpdateDisplay();
    }

    public void UpdateDisplay() {
        numDisplay.text = bitsManager.TotalBits.ToString();
    }
}
