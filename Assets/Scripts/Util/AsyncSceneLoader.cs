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
        BeginAsyncProcess(true);
    }

    public void BeginAsyncProcess(bool disableButtons) {
        if (PlayerPrefs.GetInt(TagHolder.PREF_SKIP_ANIM) == 1) {
            SceneManager.LoadScene(sceneBuildIndex, LoadSceneMode.Single);
        }
        else {
            //disables all buttons
            if (disableButtons) {
                foreach (Button button in FindObjectsOfType<Button>()) {
                    button.interactable = false;
                }
            }

            //camera animation for transition
            if (animationClip != null) {
                Animator mainCamAnim = Camera.main.GetComponent<Animator>();
                mainCamAnim.Play(animationClip.name);
            }
            
            //fade out music and begin loading
            StartCoroutine(musicManager.FadeOutAndStop(1f));
            StartCoroutine(LoadScene());
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
