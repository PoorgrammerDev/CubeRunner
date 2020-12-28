using UnityEngine;
public class GameValues : MonoBehaviour {

    //FORWARD SPEED---------
    private const float MAX_FORWARD_SPEED = 100f;

    [SerializeField]
    private float forwardSpeed = 10f;

    public float getForwardSpeed() {
        return forwardSpeed;
    }

    public void setForwardSpeed(float speed) {
        if (speed > 0 && speed < MAX_FORWARD_SPEED) {
            forwardSpeed = speed;
        }
        else {
            throw new System.Exception("Forward Speed is out of bounds.");
        }
    }

    //STRAFING SPEED---------

    [SerializeField]
    private float strafingSpeed = 10f;

    public float getStrafingSpeed() {
        return strafingSpeed;
    }

    public void setStrafingSpeed(float speed) {
        if (speed > 0) {
            strafingSpeed = speed;
        }
        else {
            throw new System.Exception("Strafing Speed cannot be negative or zero.");
        }
    }

    //THEME---------

}