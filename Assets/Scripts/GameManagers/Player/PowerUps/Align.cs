using System.Collections;
using UnityEngine;

public class Align : AbstractPowerUp {
    [SerializeField] private PlayerPowerUp powerUpManager;
    [SerializeField] private GameValues gameValues;
    [SerializeField] private CubeSpawner cubeSpawner;
    [SerializeField] private Transform player;

    [Header("Options")]
    [SerializeField] private float duration;
    [SerializeField] private bool beginningAlign;





    [SerializeField] private float[] positions; //TODO: deserialize
    [SerializeField] private int currentPosition; //TODO: deserialize

    void Start() {
        StartCoroutine(DelayStart());
    }

    IEnumerator DelayStart() {
        yield return null;
        int lanes = cubeSpawner.Lanes;
        positions = new float[lanes];

        for (int i = 0; i < lanes; i++) {
            positions[i] = (((lanes) / 2f) - i - 0.5f) * gameValues.WidthScale;
        }
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

    public IEnumerator RunAlignTimer() {
        //set active
        powerUpManager.State = PowerUpState.Active;

        FindCurrentPosition();

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
            Move(--currentPosition);
        }
    }

    public void MoveRight() {
        if (currentPosition < positions.Length - 1) {
            Move(++currentPosition);
        }
    }

    void Move(int index) {
        Vector3 position = player.position;
        position.z = this.positions[index];
        player.position = position;
    }

}