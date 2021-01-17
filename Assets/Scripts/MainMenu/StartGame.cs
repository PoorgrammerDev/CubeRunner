using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles Starting Game (Main Menu scene-side) and the UI Button
/// </summary>
public class StartGame : MonoBehaviour
{

    [SerializeField] private Material glowingMaterial;
    [SerializeField] private GameObject platform;
    [SerializeField] private PanelObjectHolder[] panels;
    [SerializeField] private Button[] buttons;
    [SerializeField] private MusicManager musicManager;

    private CubeSpin spinManager;
    private PanelObjectHolder panel;
    private new MeshRenderer renderer;
    private MenuScale menuScale;

    private bool readyToSwitchScenes = false;
    public bool ReadyToSwitchScenes {set => readyToSwitchScenes = value;}

    void Start() {
        renderer = GetComponent<MeshRenderer>();
        spinManager = GetComponent<CubeSpin>();
        menuScale = GetComponentInParent<MenuScale>();
    }

    public void Click() {
        //check for active panel
        foreach (PanelObjectHolder panel in panels) {
            if (panel.gameObject.activeInHierarchy) {
                this.panel = panel;
                break;
            }
        }

        Time.timeScale = 1f;
        renderer.material = glowingMaterial;
        menuScale.Active = false;

        //disables all buttons
        foreach (Button button in buttons) {
            button.interactable = false;
        }

        //begins loading new scene in background
        StartCoroutine(LoadScene());
        
        //removes other menu elements from screen
        panel.MoveOut();

        //music fades out
        StartCoroutine(musicManager.FadeOutAndStop(0.5f));

        //removes platform
        platform.GetComponent<Animator>().Play(TagHolder.ANIM_FADE_OUT);

        //cube stop
        spinManager.stopAtStraight();

        //cube moves closer
        StartCoroutine(CubeMovesCloser());

        //camera moves up
        Animator mainCamAnim = Camera.main.GetComponent<Animator>();
        mainCamAnim.Play(TagHolder.CAM_ANIM_START_GAME);

        //scene switching is handled by the animation event on the camera
    }

    IEnumerator CubeMovesCloser() {
        Vector3 position = transform.position;

        float magnitude = 0.01f;
        Vector3 move = new Vector3(0, 0, -magnitude);
        for (float i = 0; i < position.z; i+= magnitude) {
            transform.Translate(move, Space.World);
            yield return null;
        }

        yield break;
    }
    
    IEnumerator LoadScene() {
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(TagHolder.GAME_SCENE, LoadSceneMode.Single);
        asyncOp.allowSceneActivation = false;

        while (!asyncOp.isDone) {
            if (readyToSwitchScenes && asyncOp.progress >= 0.9f) {
                asyncOp.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
