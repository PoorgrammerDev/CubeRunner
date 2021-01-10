using UnityEngine;

/// <summary>
/// Player Movement (strafing left and right)
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;

    [SerializeField] private GameValues gameValues;

    void Awake() {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update() {
        if (!gameValues.GameActive || characterController == null || !characterController.enabled) return;

        Vector3 velocity = transform.TransformDirection(new Vector3(0f, 0f, -Input.GetAxisRaw(TagHolder.HORIZONTAL_AXIS) * gameValues.StrafingSpeed)) * Time.deltaTime;
        characterController.Move(velocity);
    }
}
