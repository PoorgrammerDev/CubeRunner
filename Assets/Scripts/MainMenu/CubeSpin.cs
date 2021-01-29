using UnityEngine;
using System.Collections;

/// <summary>
/// This class handles the Spinning Cube in the Main Menu.
/// </summary>
public class CubeSpin : MonoBehaviour
{
    [SerializeField] private int spinSpeed;
    private bool active = true;
    
    // Update is called once per frame
    void Update() {
        if (active) transform.Rotate(new Vector3(0, spinSpeed * Time.deltaTime, 0), Space.Self);
    }

    public IEnumerator StopAtStraight() {
        active = false;

        float t = 0f;
        Quaternion rotation = transform.localRotation;
        Vector3 eulerAngles = rotation.eulerAngles;

        float startY = eulerAngles.y;
        float endY = (startY % 90 != 0) ? startY + (90 - startY % 90) : startY; //finds next multiple of 90

        while (t <= 1) {
            t += 2f * Time.deltaTime;
            eulerAngles.y = Mathf.Lerp(startY, endY, t);

            rotation.eulerAngles = eulerAngles;
            transform.localRotation = rotation;
            yield return null;
        }
    }
}
