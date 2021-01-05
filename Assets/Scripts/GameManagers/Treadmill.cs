using UnityEngine;

/// <summary>
/// Makes the ground seem infinite
/// </summary>
public class Treadmill : MonoBehaviour
{

    [SerializeField] private float teleportThreshold = -175;

    [SerializeField] private GameValues gameValues;

    private new Transform transform;

    void Start() {
        transform = GetComponent<Transform>();
    }

    void Update()
    {
        if (!gameValues.GameActive) return;

        Vector3 position = transform.position;

        //check if moved backwards enough
        if (position.x < teleportThreshold) {
            position.x = 0;
        }

        //move backwards
        position.x -= gameValues.ForwardSpeed * Time.deltaTime;
        transform.position = position;
    }
}
