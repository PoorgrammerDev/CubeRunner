using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the scale of GameObjects in the menu (that can't scale automatically with the Canvas).
/// </summary>
public class MenuScale : MonoBehaviour
{
    [SerializeField] private int originalWidth = 1920;

    [SerializeField] private int originalHeight = 1080;

    [SerializeField] private float magnitude = 0.5f;

    private new Transform transform;
    private bool active = true;

    // Start is called before the first frame update
    void Start() {
        transform = gameObject.transform;
        StartCoroutine(ScheduleUpdate());
    }

    public IEnumerator ScheduleUpdate() {
        while (active && gameObject.activeInHierarchy) {
            UpdateScaling();
            yield return new WaitForSeconds(1);
        }
        yield break;
    }

    void UpdateScaling() {
        float scaleFactor = (((float) Screen.width / (float) originalWidth) + ((float) Screen.height / (float) originalHeight)) / 2f;
        Vector3 position = transform.position;
        position.z = scaleFactor * magnitude;
        transform.position = position;
    }

    public bool isActive() {
        return active;
    }

    public void setActive(bool active) {
        this.active = active;
    }
}
