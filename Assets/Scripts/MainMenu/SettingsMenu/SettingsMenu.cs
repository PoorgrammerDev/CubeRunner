using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject baseMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject tutorialMenu;
    [SerializeField] private GameObject aboutMenu;
    [SerializeField] private GameObject creditsMenu;
    [SerializeField] private ButtonSounds buttonSounds;

    //settings menu -------
    public void OpenSettingsMenu () {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        baseMenu.SetActive(true);
    }

    public void CloseSettingsMenu () {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }


    //options menu -------
    public void OpenOptionsMenu() {
        baseMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void CloseOptionsMenu() {
        optionsMenu.SetActive(false);
        baseMenu.SetActive(true);
    }

    //tutorial menu -------
    public void OpenTutorialMenu() {
        baseMenu.SetActive(false);
        settingsMenu.SetActive(false);
        tutorialMenu.SetActive(true);
    }

    public void CloseTutorialMenu() {
        tutorialMenu.SetActive(false);
        settingsMenu.SetActive(true);
        baseMenu.SetActive(true);
    }


    //about menu -------
    public void OpenAboutMenu() {
        baseMenu.SetActive(false);
        aboutMenu.SetActive(true);
    }

    public void CloseAboutMenu() {
        aboutMenu.SetActive(false);
        baseMenu.SetActive(true);
    }


    //credits menu -------
    public void OpenCreditsMenu() {
        baseMenu.SetActive(false);
        creditsMenu.SetActive(true);
    }

    public void CloseCreditsMenu() {
        creditsMenu.SetActive(false);
        baseMenu.SetActive(true);
    }


    //sounds -------
    public void PlayHoverSound() {
        buttonSounds.PlayHoverSound();
    }
    
    public void PlayClickSound() {
        buttonSounds.PlayClickSound();
    }


}
