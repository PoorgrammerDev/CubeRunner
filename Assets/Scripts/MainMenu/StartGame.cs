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

    private const int ROAD_FULLY_EXTENDED = 65;
    private const int ROAD_PASSED = -385;

    private const float SUN_RISEN = 4.5f;

    private MeshRenderer renderer;

    private Vector3 moveRight = new Vector3(0.05f, 0, 0);
    private Vector3 moveRightUI = new Vector3(9f, 0, 0);

    private Vector3 moveDown = new Vector3(0, -0.05f, 0);
    private Vector3 moveDownUI = new Vector3(0, -3f, 0);

    private const int UI_LAYER = 5;

    void Start() {
        renderer = gameObject.GetComponent<MeshRenderer>();
        spinManager = GetComponent<CubeSpin>();
    }

    public void Click() {
        renderer.material = glowingMaterial;

        //TODO disables all buttons
        foreach (Button button in buttons) {
            button.interactable = false;
        }
        
        //removes other menu elements from screen
        StartCoroutine(RemoveOtherObjects());

        //cube stop
        spinManager.stopAtStraight();

        //road extends out
        StartCoroutine(ExtendRoad());

        //camera moves up
        Animator mainCamAnim = Camera.main.GetComponent<Animator>();
        mainCamAnim.Play("StartGame");

        //countdown
        
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
        Vector3 moveVectorSlower = new Vector3(0, 0, -0.5f);
        Vector3 moveVectorIdle = new Vector3(0, 0, -0.04f);

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
        for (int i = 5; i >= -1; i--) {
            if (i > 0) {
                countdown.text = "" + i;
            }
            else {
                countdown.text = "Get Ready";
            }
            
            yield return new WaitForSecondsRealtime(1);
        }

        SceneManager.LoadScene("Scene", LoadSceneMode.Single);
        yield break;
    }



}
