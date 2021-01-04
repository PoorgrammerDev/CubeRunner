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
    private GameObject groundPlane;

    [SerializeField]
    private GameObject sun;

    [SerializeField]
    private PanelObjectHolder[] panels;
    private PanelObjectHolder panel;
    
    [SerializeField]
    private Text countdown;

    [SerializeField]
    private int countdownNum = 3;

    private const int ROAD_FULLY_EXTENDED = 65;
    private const int ROAD_PASSED = -385;

    private const float SUN_RISEN = 4.5f;

    private new MeshRenderer renderer;
    private MenuScale menuScale;
    private Button[] buttons;

    enum Direction {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    private const int UI_LAYER = 5;

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
        int horizontalLimit = 1750;
        int verticalLimit = 5;

        float minRight, minTop, maxLeft, maxBottom;
        do {
            maxLeft = moveObjects(panel.LeftScreenObjects, Direction.LEFT, 0.05f, 150);
            minRight = moveObjects(panel.RightScreenObjects, Direction.RIGHT, 0.05f, 150);
            minTop = moveObjects(panel.TopScreenObjects, Direction.UP, 0.05f, 150);
            maxBottom = moveObjects(panel.BottomScreenObjects, Direction.DOWN, 0.05f, 150);

            yield return null;
        } while (maxLeft > -horizontalLimit || minRight < horizontalLimit || maxBottom > -verticalLimit || minTop < verticalLimit);

        panel.DeactivateAll();
        yield break;
    }

    //dear lord this function is so poorly written but right now i can't figure out a better way
    //TODO clean this mess of a function up
    float moveObjects(Transform[] screenObjects, Direction direction, float moveSpeed, float UIAmplifier) {
        bool up = direction.Equals(Direction.UP);
        bool down = direction.Equals(Direction.DOWN);
        bool left = direction.Equals(Direction.LEFT);
        bool right = direction.Equals(Direction.RIGHT);

        float extrema = (down || left) ? float.MinValue : float.MaxValue;
        //forming correct vector with regard to direction
        Vector3 moveVector;
        if (left || right) {
            moveVector = new Vector3(moveSpeed, 0, 0);
        }
        else {
            moveVector = new Vector3(0, moveSpeed, 0);
        }
        if (down || left) moveVector *= -1;


        foreach (Transform screenObject in screenObjects) {
            //moving each object
            screenObject.Translate((screenObject.gameObject.layer == UI_LAYER) ? (moveVector * UIAmplifier) : moveVector, Space.World);

            if (up && screenObject.localPosition.y < extrema) {
                extrema = screenObject.localPosition.y;
            }
            else if (down && screenObject.localPosition.y > extrema) {
                extrema = screenObject.localPosition.y;
            }
            else if (left && screenObject.localPosition.x > extrema) {
                extrema = screenObject.localPosition.x;
            }
            else if (screenObject.localPosition.x < extrema) {
                extrema = screenObject.localPosition.x;
            }
        }
        return extrema;
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

    //IEnumerator Sunrise() {
    //    sun.SetActive(true);
//
    //    Transform transform = sun.transform;
    //    Vector3 moveVector = new Vector3(0, 0.01f, 0);
//
    //    StartCoroutine(FadeInCountdown());
    //    do {
    //        transform.Translate(moveVector, Space.World);
    //        yield return null;
    //    } while (transform.position.y < SUN_RISEN);
    //}

    //IEnumerator FadeInCountdown() {
    //    countdown.gameObject.SetActive(true);
    //    countdown.text = countdownNum.ToString();
//
    //    //fade in
    //    Color color = countdown.color;
    //    for (float i = 0; i <= 1; i += 0.05f) {
    //        color.a = i;
    //        countdown.color = color;
    //        yield return null;
    //    }
//
    //    StartCoroutine(StartCountingDown());
    //}
//
    //IEnumerator StartCountingDown() {
    //    for (int i = countdownNum; i > 0; i--) {
    //        countdown.text = i.ToString();
    //        yield return new WaitForSecondsRealtime(1);
    //    }
//
    //    countdown.text = "Get Ready";
    //    yield return new WaitForSecondsRealtime(0.75f);
//
    //    //fade out
    //    Color color = countdown.color;
    //    for (float i = 1; i >= 0; i -= 0.05f) {
    //        color.a = i;
    //        countdown.color = color;
    //        yield return null;
    //    }
//
    //    countdown.gameObject.SetActive(false);
    //    yield return new WaitForSecondsRealtime(0.125f);
    //    SceneManager.LoadScene(TagHolder.GAME_SCENE, LoadSceneMode.Single);
    //}



}
