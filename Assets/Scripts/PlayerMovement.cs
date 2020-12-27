using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;

    [SerializeField]
    private float strafeSpeed;

    [SerializeField]
    private float gravity;

    [SerializeField]
    private float jumpForce;

    private float verticalVelocity;

    void Awake() {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vel = transform.TransformDirection(new Vector3(0f, 0f, -Input.GetAxis("Horizontal") * strafeSpeed)) * Time.deltaTime;
        vel = ApplyGravity(vel);

        characterController.Move(vel);
    }

    Vector3 ApplyGravity(Vector3 velocity) {
        if (characterController.isGrounded) {
            velocity = Jump(velocity);
        }

        verticalVelocity -= gravity * Time.deltaTime;
        velocity.y = verticalVelocity * Time.deltaTime;
        return velocity;
    }

    Vector3 Jump(Vector3 velocity) {
        if (Input.GetKeyDown(KeyCode.Space)) {
            verticalVelocity = jumpForce;
        }
        return velocity;
    }
}
