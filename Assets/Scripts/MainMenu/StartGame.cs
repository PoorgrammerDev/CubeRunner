using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{

    [SerializeField]
    private Material glowingMaterial;

    private CubeSpin spinManager;

    [SerializeField]
    private Transform[] rightScreenObjects;

    [SerializeField]
    private Transform[] bottomScreenObjects;

    [SerializeField]
    private Button[] buttons;

    [SerializeField]
    private GameObject groundPlane;

    [SerializeField]
    private GameObject sun;

    [SerializeField]
    private Text countdown;

    [SerializeField]
    private int countdownNum = 3;

    private const int ROAD_FULLY_EXTENDED = 65;
    private const int ROAD_PASSED = -385;

    private const float SUN_RISEN = 4.5f;

    private new MeshRenderer renderer;
    private MenuScale menuScale;

    private Vector3 moveRight = new Vector3(0.05f, 0, 0);
    private Vector3 moveRightUI = new Vector3(9f, 0, 0);

    private Vector3 moveDown = new Vector3(0, -0.05f, 0);
    private Vector3 moveDownUI = new Vector3(0, -3f, 0);

    private const int UI_LAYER = 5;

    void Start() {
        renderer = GetComponent<MeshRenderer>();
        spinManager = GetComponent<CubeSpin>();
        menuScale = GetComponent<MenuScale>();
    }

    public void Click() {
        renderer.material = glowingMaterial;
        menuScale.setActive(false);

        //isables all buttons
        foreach (Button button in buttons) {
            button.interactable = false;
        }
        
        //removes other menu elements from screen
        StartCoroutine(RemoveOtherObjects());

        //cube stop
        spinManager.stopAtStraight();

        //road extends out
        StartCoroutine(ExtendRoad());
        StartCoroutine(CubeMovesCloser());

        //camera moves up
        Animator mainCamAnim = Camera.main.GetComponent<Animator>();
        mainCamAnim.Play(TagHolder.CAM_ANIM_START_GAME);
    }

    IEnumerator RemoveOtherObjects () {
        float minRight = float.MaxValue;
        float maxBottom = float.MinValue;

        do {
            minRight = float.MaxValue;
            maxBottom = float.MinValue;
            
            foreach (Transform rightScreenObject in rightScreenObjects) {
                rightScreenObject.Translate((rightScreenObject.gameObject.layer == UI_LAYER) ? moveRightUI : moveRight, Space.World);

                if (rightScreenObject.localPosition.x < minRight) {
                    minRight = rightScreenObject.localPosition.x;
                }
            }

            foreach (Transform bottomScreenObject in bottomScreenObjects) {
                bottomScreenObject.Translate((bottomScreenObject.gameObject.layer == UI_LAYER) ? moveDownUI : moveDown, Space.World);

                if (bottomScreenObject.localPosition.y > maxBottom) {
                    maxBottom = bottomScreenObject.localPosition.y;
                }
            }

            yield return null;
        } while (minRight < 2000 || maxBottom > -50);

        foreach(Transform rightScreenObject in rightScreenObjects) {
            rightScreenObject.gameObject.SetActive(false);
        }
        foreach(Transform bottomScreenObject in bottomScreenObjects) {
            bottomScreenObject.gameObject.SetActive(false);
        }

        yield break;
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
                StartCoroutine(Sunrise());
                sunAlreadyRising = true;
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

    IEnumerator Sunrise() {
        sun.SetActive(true);

        Transform transform = sun.transform;
        Vector3 moveVector = new Vector3(0, 0.01f, 0);

        StartCoroutine(FadeInCountdown());
        do {
            transform.Translate(moveVector, Space.World);
            yield return null;
        } while (transform.position.y < SUN_RISEN);

        yield break;
    }

    IEnumerator FadeInCountdown() {
        countdown.gameObject.SetActive(true);
        countdown.text = countdownNum.ToString();

        Color color = countdown.color;
        for (float i = 0; i <= 1; i += 0.05f) {
            color.a = i;
            countdown.color = color;
            yield return null;
        }

        StartCoroutine(StartCountingDown());
        yield break;
    }

    IEnumerator StartCountingDown() {
        for (int i = countdownNum; i >= -1; i--) {
            if (i > 0) {
                countdown.text = i.ToString();
            }
            else {
                countdown.text = "Get Ready";
            }
            
            yield return new WaitForSecondsRealtime(1);
        }

        SceneManager.LoadScene(TagHolder.GAME_SCENE, LoadSceneMode.Single);
        yield break;
    }



}
