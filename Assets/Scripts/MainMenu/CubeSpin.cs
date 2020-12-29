using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpin : MonoBehaviour
{
    [SerializeField]
    private float spinSpeed = 0.5f;

    private Transform transform;

    // Start is called before the first frame update
    void Start() {
        transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update() {
        transform.Rotate(new Vector3(0, spinSpeed, 0), Space.Self);
    }
}
