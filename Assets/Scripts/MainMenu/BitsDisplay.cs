using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Class is meant to be placed directly on UI Text. Updates its value to match the current Total Bits.
/// </summary>
public class BitsDisplay : MonoBehaviour
{
    [SerializeField] private SaveManager saveManager;

    private TextMeshProUGUI numDisplay;

    // Start is called before the first frame update
    void Start() {
        numDisplay = GetComponent<TextMeshProUGUI>();
        UpdateDisplay();
    }

    public void UpdateDisplay() {
        numDisplay.text = saveManager.TotalBits.ToString();
    }

    public IEnumerator TransitionDisplay(float speed) {
        int value;
        if (int.TryParse(numDisplay.text, out value)) {
            if (value != saveManager.TotalBits) {
                float t = 0.0f;
                while (t <= 1.0f) {
                    t += speed * Time.deltaTime;
                    numDisplay.text = ((int) Mathf.Lerp(value, saveManager.TotalBits, t)).ToString();

                    yield return null;
                }
            }
        }
    }
}
