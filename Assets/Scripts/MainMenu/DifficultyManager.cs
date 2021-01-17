using UnityEngine;
using TMPro;

/// <summary>
/// Class to modify Difficulty value of the game, and also manages the Difficulty Menu.
/// </summary>
public class DifficultyManager : MonoBehaviour
{
    private Difficulty difficulty;
    public Difficulty Difficulty {get => difficulty; set {
            difficulty = (Difficulty) value;
            PlayerPrefs.SetInt(TagHolder.PREF_DIFFICULTY, (int) value);
        }
    }

    [SerializeField] private Difficulty defaultDifficulty;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Color[] colors = {
        new Color(0.56f, 0.75f, 0.43f), //easy - green
        new Color(0.98f, 0.78f, 0.31f), //normal - yellow
        new Color(0.98f, 0.25f, 0.27f), //hard - red
        new Color(0.69f, 0.17f, 0.75f), //impossible - purple
    };
    
    void Start() {
        difficulty = PlayerPrefs.HasKey(TagHolder.PREF_DIFFICULTY) ?
        (Difficulty) PlayerPrefs.GetInt(TagHolder.PREF_DIFFICULTY) :
        defaultDifficulty;

        UpdateText();
    }

    public void ChangeDifficulty() {
        //swap around diffs, ensures looping around
        Difficulty = ((int) Difficulty >= 3) ? 0 : ++Difficulty;
        
        UpdateText();
    }

    void UpdateText() {
        text.text = Difficulty.ToString();
        //text.CrossFadeColor(colors[(int) Difficulty], 0.25f, true, false); //disabled because it didn't actually change the vertex color for some reason and caused the color to reset to white when changing menu
        text.color = colors[(int) Difficulty];
    }
}
