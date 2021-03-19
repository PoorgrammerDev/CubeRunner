using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Align : AbstractPowerUp {
    [SerializeField] private PlayerPowerUp powerUpManager;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameValues gameValues;
    [SerializeField] private CubeSpawner cubeSpawner;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject targetPrefab;
    public Transform targetParentObj;
    [SerializeField] private GameObject[] targets;
    public GameObject leftMoveArrow;
    public GameObject rightMoveArrow;

    [SerializeField] private Transform playerShadowsParent;
    [SerializeField] private GameObject shadowPrefab;
    private Queue<GameObject> shadows;

    [Header("Options")]
    [SerializeField] private float duration;
    [SerializeField] private bool beginningAlign;
    [SerializeField] private int shadowCount;


    private float[] positions;
    private int currentPosition;

    void Start() {
        StartCoroutine(DelayStart());
        MakeShadows(shadowCount * 2);
    }

    IEnumerator DelayStart() {
        yield return null;
        int lanes = cubeSpawner.Lanes;
        positions = new float[lanes];

        //calculate positions
        for (int i = 0; i < lanes; i++) {
            positions[i] = (((lanes) / 2f) - i - 0.5f) * gameValues.WidthScale;
        }

        //initalize targets
        targets = new GameObject[lanes];
        Quaternion rotation = Quaternion.Euler(90, 90, 0);
        for (int i = 0; i < lanes; i++) {
            targets[i] = Instantiate(targetPrefab, new Vector3(0, 0.1f, positions[i]), rotation, targetParentObj);
        }

        //set move arrow positions
        Vector3 position = leftMoveArrow.transform.position;
        position.z = gameValues.WidthScale;
        leftMoveArrow.transform.position = position;
        
        position.z = -gameValues.WidthScale;
        rightMoveArrow.transform.position = position;
    }

    void FindCurrentPosition() {
        int index = -1;
        float min = int.MaxValue;
        float playerZ = player.position.z;

        float tester;
        for (int i = 0; i < positions.Length; i++) {
            tester = Mathf.Abs(playerZ - positions[i]);
            if (tester < min) {
                min = tester;
                index = i;
            }
        }

        if (index != -1) {
            currentPosition = index;
            if (beginningAlign) {
                Move(index);
            }
        }
    }

    void UpdateTargets() {
        leftMoveArrow.SetActive(currentPosition > 0);
        rightMoveArrow.SetActive(currentPosition < positions.Length - 1);
    }

    public IEnumerator RunAlign() {
        //set active
        powerUpManager.State = PowerUpState.Active;

        FindCurrentPosition();
        targetParentObj.gameObject.SetActive(true);

        //wait
        powerUpManager.ticker = duration;
        while (powerUpManager.ticker > 0) {
            powerUpManager.TopBar.value = (powerUpManager.ticker / duration);
            yield return null;
        }

        powerUpManager.RemovePowerUp();
    }

    public void MoveLeft() {
        if (currentPosition > 0) {
            Move(currentPosition - 1);
        }
    }

    public void MoveRight() {
        if (currentPosition < positions.Length - 1) {
            Move(currentPosition + 1);
        }
    }

    void MakeShadows(int count) {
        Vector3 pos = new Vector3(0, 0.6f, 0);
        Quaternion rot = new Quaternion();
        shadows = new Queue<GameObject>();
        for (int i = 0; i < count; i++) {
            shadows.Enqueue(Instantiate(shadowPrefab, pos, rot, playerShadowsParent));
        }
    }

    GameObject GetShadow() {
        if (shadows.Count > 0) {
            return shadows.Dequeue();
        }
        MakeShadows(1);
        return GetShadow();
    }

    void Move(int index) {
        //teleport logic
        Vector3 position = player.position;
        Vector3 lastPosition = position;
        position.z = this.positions[index];
        player.position = position;
        currentPosition = index;
        UpdateTargets();

        //sfx
        audioSource.clip = Sounds[0];
        audioSource.time = SoundStartTimes[0];
        audioSource.Play();

        //vfx
        for (int i = 0; i < shadowCount; i++) {
            StartCoroutine(ShadowMove(GetShadow(), lastPosition, position, (shadowCount - i) + 3));
        }
    }

    IEnumerator ShadowMove(GameObject shadow, Vector3 lastPosition, Vector3 position, float speed) {
        float t = 0f;
        shadow.SetActive(true);
        while (t <= 1f) {
            t += speed * Time.deltaTime;
            shadow.transform.position = Vector3.Lerp(lastPosition, position, t);
            yield return null;
        }

        shadow.SetActive(false);
        shadows.Enqueue(shadow);
    }

}