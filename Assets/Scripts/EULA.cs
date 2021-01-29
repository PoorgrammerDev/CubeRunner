using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EULA : MonoBehaviour
{
    [SerializeField] private GameObject eulaPanel;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private TextAsset eulaTextAsset;
    [SerializeField] private TextMeshProUGUI eulaText;
    [SerializeField] private Toggle readTermsCheckbox;

    // Start is called before the first frame update
    void Start() {
        if (PlayerPrefs.GetInt(TagHolder.PREF_HAS_ACCEPTED_EULA) == 1) return;

        mainMenu.SetActive(false);
        eulaPanel.SetActive(true);
        eulaText.text = eulaTextAsset.text;
    }

    public void Accept() {
        if (readTermsCheckbox.isOn) {
            PlayerPrefs.SetInt(TagHolder.PREF_HAS_ACCEPTED_EULA, 1);
            eulaPanel.SetActive(false);
            mainMenu.SetActive(true);
        }
    }

    public void Decline() {
        Application.Quit();
    }
}
