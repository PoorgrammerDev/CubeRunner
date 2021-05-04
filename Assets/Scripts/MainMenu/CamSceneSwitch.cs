using UnityEngine;

public class CamSceneSwitch : MonoBehaviour
{
    [SerializeField] private StartGame startGame;
    [SerializeField] private AsyncSceneLoader enterUpgrades;
    [SerializeField] private AsyncSceneLoader exitUpgrades;
    void ChangeGameScene() {
        if (startGame != null) startGame.ReadyToSwitchScenes = true;
    }

    void ChangeUpgradeScene() {
        if (enterUpgrades != null) enterUpgrades.ActivateScene();
    }

    void FromUpgradesToMM() {
        if (exitUpgrades == null) return; 

        GameObject gameObject = new GameObject("RETURN_FROM_UPG");
        gameObject.AddComponent<ReturnFromUpg>();

        DontDestroyOnLoad(gameObject);
        exitUpgrades.ActivateScene();
    }
    
}
