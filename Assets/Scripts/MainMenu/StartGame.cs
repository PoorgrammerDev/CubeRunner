using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles Starting Game and the UI Button
/// </summary>
public class StartGame : MonoBehaviour
{

    [SerializeField] private Material glowingMaterial;
    [SerializeField] private GameObject groundPlane;
    [SerializeField] private GameObject sun;
    [SerializeField] private PanelObjectHolder[] panels;

    private CubeSpin spinManager;
    private PanelObjectHolder panel;
    private new MeshRenderer renderer;
    private MenuScale menuScale;
    private Button[] buttons;

    private const int ROAD_FULLY_EXTENDED = 65;
    private const int ROAD_PASSED = -385;

    void Start() {
        renderer = GetComponent<MeshRenderer>();
        spinManager = GetComponent<CubeSpin>();
        menuScale = GetComponentInParent<MenuScale>();

        buttons = GameObject.FindObjectsOfType<Button>();
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
        menuScale.setActive(false);

        //disables all buttons
        foreach (Button button in buttons) {
            button.interactable = false;
        }
        
        //removes other menu elements from screen
        panel.MoveOut();

        //cube stop
        spinManager.stopAtStraight();

        //road extends out
        StartCoroutine(ExtendRoad());
        StartCoroutine(CubeMovesCloser());

        //camera moves up
        Animator mainCamAnim = Camera.main.GetComponent<Animator>();
        mainCamAnim.Play(TagHolder.CAM_ANIM_START_GAME);
    }

    IEnumerator ExtendRoad() {
        groundPlane.SetActive(true);

        Transform transform = groundPlane.transform;
        Vector3 moveVector = new Vector3(0, 0, -1f);
        Vector3 moveVectorSlower = new Vector3(0, 0, -0.3f);
        Vector3 moveVectorIdle = new Vector3(0, 0, -0.075f);

        bool sunAlreadyRising = false;
        do {
            Vector3 vector = moveVectorIdle;
            if (transform.position.z > (ROAD_FULLY_EXTENDED + 100)) {
                vector = moveVector;
            }
            else if (transform.position.z > ROAD_FULLY_EXTENDED) {
                vector = moveVectorSlower;
            }
            else if (!sunAlreadyRising) {
                sun.SetActive(true);
                sunAlreadyRising = true;
            }
            else {
                StartCoroutine(ToGameScene());
            }

            transform.Translate(vector, Space.World);
            yield return null;
        } while (transform.position.z > ROAD_PASSED);

        groundPlane.SetActive(false);
        yield break;
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

    IEnumerator ToGameScene() {
        yield return new WaitForSecondsRealtime(1);
        SceneManager.LoadScene(TagHolder.GAME_SCENE, LoadSceneMode.Single);
    }
}
