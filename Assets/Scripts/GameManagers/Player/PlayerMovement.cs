using UnityEngine;

/// <summary>
/// Player Movement (strafing left and right)
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;
    private float verticalVelocity;
    private int mobileAxis;

    [SerializeField] private GameValues gameValues;
    [SerializeField] private GameObject mobileControls;
    [SerializeField] private float gravity;

    void Awake() {
        characterController = GetComponent<CharacterController>();

        #if UNITY_IOS || UNITY_ANDROID
            mobileControls.SetActive(true);
        #endif
    }

    // Update is called once per frame
    void Update() {
        if (!gameValues.GameActive || characterController == null || !characterController.enabled) return;

        float horizontal;

        #if UNITY_STANDALONE || UNITY_WEBGL
            horizontal = Input.GetAxisRaw(TagHolder.HORIZONTAL_AXIS);
        #endif

        #if UNITY_ANDROID || UNITY_IOS
            horizontal = mobileAxis;
        #endif


        Vector3 transformDirection = new Vector3(0f, 0f, -horizontal * gameValues.StrafingSpeed);
        Vector3 velocity = transform.TransformDirection(transformDirection) * Time.deltaTime;
        velocity = ApplyGravity(velocity);
        characterController.Move(velocity);
    }

     Vector3 ApplyGravity(Vector3 velocity) {
        verticalVelocity -= gravity * Time.deltaTime;
        velocity.y = verticalVelocity * Time.deltaTime;
        return velocity;
    }

    public void mobileMoveLeft() {
        mobileAxis = -1;
    }

    public void mobileMoveRight() {
        mobileAxis = 1;
    }

    public void mobileRelease() {
        mobileAxis = 0;
    }
}
