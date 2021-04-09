using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterUpgrades : MonoBehaviour
{
    public void Enter() {
        SceneManager.LoadScene(TagHolder.UPGRADES_SCENE, LoadSceneMode.Single);
    }
}
