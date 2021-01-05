using UnityEngine;

/// <summary>
/// Player Movement (strafing left and right)
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;

    [SerializeField] private GameValues gameValues;

    //[SerializeField] private float gravity;

    //[SerializeField] private float jumpForce;

    //private float verticalVelocity;

    void Awake() {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameValues.GameActive || characterController == null || !characterController.enabled) return;

        Vector3 velocity = transform.TransformDirection(new Vector3(0f, 0f, -Input.GetAxisRaw(TagHolder.HORIZONTAL_AXIS) * gameValues.StrafingSpeed)) * Time.deltaTime;
        //velocity = ApplyGravity(vel);

        characterController.Move(velocity);
    }

    //Vector3 ApplyGravity(Vector3 velocity) {
    //    if (characterController.isGrounded) {
    //        velocity = Jump(velocity);
    //    }
//
    //    verticalVelocity -= gravity * Time.deltaTime;
    //    velocity.y = verticalVelocity * Time.deltaTime;
    //    return velocity;
    //}

    //Vector3 Jump(Vector3 velocity) {
    //    if (Input.GetKeyDown(KeyCode.Space)) {
    //        verticalVelocity = jumpForce;
    //    }
    //    return velocity;
    //}
}
