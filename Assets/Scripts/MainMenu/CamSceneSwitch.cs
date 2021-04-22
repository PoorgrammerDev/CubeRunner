using UnityEngine;

public class CamSceneSwitch : MonoBehaviour
{
    [SerializeField] private StartGame startGame;
    [SerializeField] private EnterUpgrades upgrades;
    [SerializeField] private ShopManager shopManager;
    void ChangeGameScene() {
        if (startGame != null) startGame.ReadyToSwitchScenes = true;
    }

    void ChangeUpgradeScene() {
        if (upgrades != null) upgrades.SwitchScenes();
    }

    void FromUpgradesToMM() {
        if (shopManager == null) return; 

        GameObject gameObject = new GameObject("RETURN_FROM_UPG");
        gameObject.AddComponent<ReturnFromUpg>();

        DontDestroyOnLoad(gameObject);
        shopManager.ChangeSceneToMM();
    }
    
}
