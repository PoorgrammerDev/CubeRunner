using UnityEngine;

public class TutorialMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] panels;
    private int currentPanelIndex;

    // Start is called before the first frame update
    void Start()
    {
        currentPanelIndex = 0;
        panels[currentPanelIndex].SetActive(true);
    }

    public void NextPanel(bool reverse) {
        //disable current panel
        panels[currentPanelIndex].SetActive(false);
        
        //increment / decrement value, ensuring wrapping
        if (reverse) {
            if (--currentPanelIndex < 0) currentPanelIndex = panels.Length - 1;
        } 
        else {
            if (++currentPanelIndex >= panels.Length) currentPanelIndex = 0;
        }

        //enable new panel
        panels[currentPanelIndex].SetActive(true);
    }
}
