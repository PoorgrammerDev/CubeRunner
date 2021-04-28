using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnterUpgrades : MonoBehaviour
{
    [SerializeField] private MusicManager musicManager;

    public void Enter() {
        if (PlayerPrefs.GetInt(TagHolder.PREF_SKIP_ANIM) == 1) {
            SwitchScenes();
        }
        else {
            //disables all buttons
            foreach (Button button in FindObjectsOfType<Button>()) {
                button.interactable = false;
            }

            //music fades out
            StartCoroutine(musicManager.FadeOutAndStop(1f));

            Animator mainCamAnim = Camera.main.GetComponent<Animator>();
            mainCamAnim.Play(TagHolder.CAM_UPGRADES_ENTER);
        }
    }

    public void SwitchScenes() {
        SceneManager.LoadScene(TagHolder.UPGRADES_SCENE, LoadSceneMode.Single);
    }
}
