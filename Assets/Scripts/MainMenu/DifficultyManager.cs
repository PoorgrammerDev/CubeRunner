﻿using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class to modify Difficulty value of the game, and also manages the Difficulty Menu.
/// </summary>
public class DifficultyManager : MonoBehaviour
{
    private Difficulty difficulty;
    public Difficulty Difficulty => difficulty;

    [SerializeField] private Difficulty defaultDifficulty;

    [SerializeField] private GameObject[] difficultyButtons;

    [SerializeField] private GameObject mainMenuScreen;

    [SerializeField] private GameObject difficultyScreen;

    [SerializeField] private Color[] selectedItemColors = {
        new Color(0.56f, 0.75f, 0.43f), //easy - green
        new Color(0.98f, 0.78f, 0.31f), //normal - yellow
        new Color(0.98f, 0.25f, 0.27f), //hard - red
        new Color(0.69f, 0.17f, 0.75f), //impossible - purple
    };
    
    [SerializeField] private Color unselectedItemColor = Color.white;
    
    void Awake() {
        difficulty = PlayerPrefs.HasKey(TagHolder.PREF_DIFFICULTY) ?
        (Difficulty) PlayerPrefs.GetInt(TagHolder.PREF_DIFFICULTY) :
        defaultDifficulty;
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
                text.CrossFadeColor(selectedItemColors[i], 0.5f, true, false);
                buttonImage.CrossFadeColor(selectedItemColors[i], 0.5f, true, false);

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

    public Color GetCurrentColor() {
        return selectedItemColors[(int) difficulty];
    }
}