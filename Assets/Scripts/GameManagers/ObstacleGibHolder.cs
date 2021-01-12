using UnityEngine;

public class ObstacleGibHolder : MonoBehaviour
{   
    [SerializeField] private GameValues gameValues;

    // Update is called once per frame
    void Update() {
        if (!gameValues.GameActive) return;

        Vector3 position = transform.position;
        position.x -= gameValues.ForwardSpeed * Time.deltaTime;
        transform.position = position;
    }
}
