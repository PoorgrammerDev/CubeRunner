using UnityEngine;

/// <summary>
/// Makes the ground seem infinite
/// </summary>
public class Treadmill : MonoBehaviour
{
    [SerializeField] public bool active;

    [SerializeField] private float speed = 1;
    public float Speed {get => speed; set {
            if (value > 0) {
                speed = value;
            }
        }
    }

    [SerializeField] private float teleportThreshold = -175;
    [SerializeField] private GameValues gameValues;

    void Update()
    {
        if (!active) return;

        Vector3 position = transform.position;

        //check if moved backwards enough
        if (position.x < teleportThreshold) {
            position.x = 0;
        }

        //move backwards
        position.x -= gameValues.ForwardSpeed * speed * Time.deltaTime;
        transform.position = position;
    }
}
