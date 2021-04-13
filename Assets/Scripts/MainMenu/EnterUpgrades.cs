using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterUpgrades : MonoBehaviour
{
    [SerializeField] private MusicManager musicManager;

    public void Enter() {
        if (PlayerPrefs.GetInt(TagHolder.PREF_SKIP_ANIM) == 1) {
            SwitchScenes();
        }
        else {
            //music fades out
            StartCoroutine(musicManager.FadeOutAndStop(0.5f));

            Animator mainCamAnim = Camera.main.GetComponent<Animator>();
            mainCamAnim.Play(TagHolder.CAM_UPGRADES_ENTER);
        }
    }

    public void SwitchScenes() {
        SceneManager.LoadScene(TagHolder.UPGRADES_SCENE, LoadSceneMode.Single);
    }
}
