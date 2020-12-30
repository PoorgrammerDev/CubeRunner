using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpin : MonoBehaviour
{
    [SerializeField]
    private bool active = true;

    [SerializeField]
    private bool triggerStopAtStraight = false;

    [SerializeField]
    private float spinSpeed = 0.5f;

    private new Transform transform;

    // Start is called before the first frame update
    void Start() {
        transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update() {
        if (active) {
            Quaternion rotation = transform.localRotation;
            Vector3 eulerAngles = rotation.eulerAngles;
            if (triggerStopAtStraight && (int) eulerAngles.y % 90 == 0) {
                triggerStopAtStraight = false;
                active = false;

                eulerAngles.y = (int) eulerAngles.y;
                rotation.eulerAngles = eulerAngles;
                transform.localRotation = rotation;
                return;
            }

            transform.Rotate(new Vector3(0, spinSpeed, 0), Space.Self);
        }
    }

    public bool isSpinning() {
        return active;
    }

    public void setSpinning(bool spin) {
        active = spin;
    }

    public void stopAtStraight() {
        triggerStopAtStraight = true;
    }

    /* NOTE: couldn't get this to work, just using scheduled stop w/o slowdown for the time being
    public IEnumerator slowDownAndStop(int slowingRate, float delay) {
        stopAtStraight();
        float rotUntilNextStraight = 90 - (transform.localEulerAngles.y % 90);
        print(rotUntilNextStraight);

        do {
            if (rotUntilNextStraight < 15) {
                float spinSpeedClone = spinSpeed;
                spinSpeedClone /= (float) slowingRate;

                if (spinSpeedClone > 0) {
                    spinSpeed = spinSpeedClone;
                }
                yield return null;
            }

            yield return null;
        } while (active);
        yield break;
    }
    */
}
