using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AsyncSceneLoader : MonoBehaviour
{
    [SerializeField] private int sceneBuildIndex;
    [SerializeField] private AnimationClip animationClip;
    [SerializeField] private MusicManager musicManager;
    private bool readyToSwitchScenes = false;

    public void BeginAsyncProcess() {
        if (PlayerPrefs.GetInt(TagHolder.PREF_SKIP_ANIM) == 1) {
            SceneManager.LoadScene(sceneBuildIndex, LoadSceneMode.Single);
        }
        else {
            //disables all buttons
            foreach (Button button in FindObjectsOfType<Button>()) {
                button.interactable = false;
            }

            StartCoroutine(LoadScene());

            //music fades out
            StartCoroutine(musicManager.FadeOutAndStop(1f));

            Animator mainCamAnim = Camera.main.GetComponent<Animator>();
            mainCamAnim.Play(animationClip.name);
        }
    }

    private IEnumerator LoadScene() {
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Single);
        asyncOp.allowSceneActivation = false;

        while (!asyncOp.allowSceneActivation) {
            if (readyToSwitchScenes && asyncOp.progress >= 0.9f) {
                asyncOp.allowSceneActivation = true;
                break;
            }
            yield return null;
        }
    }

    public void ActivateScene() {
        readyToSwitchScenes = true;
    }
}
