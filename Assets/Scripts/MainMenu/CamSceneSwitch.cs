using UnityEngine;

public class CamSceneSwitch : MonoBehaviour
{
    [SerializeField] private StartGame startGame;
    void ChangeGameScene() {
        startGame.ReadyToSwitchScenes = true;
    }
    
}
