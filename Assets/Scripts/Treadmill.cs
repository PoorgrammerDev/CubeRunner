﻿using UnityEngine;

public class Treadmill : MonoBehaviour
{

    [SerializeField]
    private float teleportThreshold = -175;

    [SerializeField]
    private GameValues gameValues;

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
        position.x -= gameValues.getForwardSpeed() * Time.deltaTime;
        transform.position = position;
    }
}