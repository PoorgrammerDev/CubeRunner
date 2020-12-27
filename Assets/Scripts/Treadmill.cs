using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treadmill : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private float teleportThreshold = -149;

    private Transform transform;

    void Start() {
        transform = GetComponent<Transform>();
    }

    void Update()
    {
        Vector3 position = transform.position;

        //check if moved backwards enough
        if (position.x < teleportThreshold) {
            position.x = 0;
        }

        //move backwards
        position.x -= speed * Time.deltaTime;
        transform.position = position;
    }
}
