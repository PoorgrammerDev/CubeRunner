using UnityEngine;
using UnityEngine.UI;

public class DifficultyManager : MonoBehaviour
{
    [SerializeField]
    private Difficulty difficulty;
    public Difficulty Difficulty => difficulty;

    [SerializeField]
    private GameObject[] difficultyButtons;

    [SerializeField]
    private GameObject mainMenuScreen;

    [SerializeField]
    private GameObject difficultyScreen;

    private Color selectedItemColor = new Color(1f, 0.831f, 0.098f);
    private Color unselectedItemColor = Color.white;
    
    void Awake() {
        difficulty = (Difficulty) PlayerPrefs.GetInt(TagHolder.PREF_DIFFICULTY);
    }

    public void SetDifficulty(int value) {
        difficulty = (Difficulty) value;
        PlayerPrefs.SetInt(TagHolder.PREF_DIFFICULTY, value);
        UpdateMenu();
    }

    void UpdateMenu() {
        for (int i = 0; i < difficultyButtons.Length; i++) {
            Image buttonImage = difficultyButtons[i].GetComponent<Image>();
            Text text = buttonImage.GetComponentInChildren<Text>();

            if (i == (int) difficulty) {
                text.CrossFadeColor(selectedItemColor, 0.5f, true, false);
                buttonImage.CrossFadeColor(selectedItemColor, 0.5f, true, false);
            }
            else {
                text.CrossFadeColor(unselectedItemColor, 0.5f, true, false);
                buttonImage.CrossFadeColor(unselectedItemColor, 0.5f, true, false);
            }
        }
    }

    public void ChangeToDifficultyMenu(bool reverse) {
        mainMenuScreen.SetActive(reverse);
        difficultyScreen.SetActive(!reverse);

        if (!reverse) UpdateMenu();
    }
}
