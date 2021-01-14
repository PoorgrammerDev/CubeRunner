using UnityEngine;

/// <summary>
/// Player Movement (strafing left and right)
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;
    private float verticalVelocity;

    [SerializeField] private GameValues gameValues;
    [SerializeField] private float gravity;

    void Awake() {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update() {
        if (!gameValues.GameActive || characterController == null || !characterController.enabled) return;

        Vector3 velocity = transform.TransformDirection(new Vector3(0f, 0f, -Input.GetAxisRaw(TagHolder.HORIZONTAL_AXIS) * gameValues.StrafingSpeed)) * Time.deltaTime;
        velocity = ApplyGravity(velocity);
        characterController.Move(velocity);
    }

     Vector3 ApplyGravity(Vector3 velocity) {
        verticalVelocity -= gravity * Time.deltaTime;
        velocity.y = verticalVelocity * Time.deltaTime;
        return velocity;
    }
}
