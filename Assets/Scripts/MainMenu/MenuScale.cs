using UnityEngine;
using System.Collections;

public class MenuScale : MonoBehaviour
{
    private new Transform transform;
    
    [SerializeField]
    private int originalWidth = 1920;

    [SerializeField]
    private int originalHeight = 1080;

    [SerializeField]
    private float magnitude = 0.5f;

    private bool active = true;

    // Start is called before the first frame update
    void Start() {
        transform = gameObject.transform;

        StartCoroutine(UpdateScaling());
    }

    public IEnumerator UpdateScaling() {
        while (active && gameObject.activeInHierarchy) {
            float scaleFactor = (((float) Screen.width / (float) originalWidth) + ((float) Screen.height / (float) originalHeight)) / 2f;
            Vector3 position = transform.position;
            position.z = scaleFactor * magnitude;
            transform.position = position;
            yield return new WaitForSeconds(1);
        }
        yield break;
    }

    public bool isActive() {
        return active;
    }

    public void setActive(bool active) {
        this.active = active;
    }
}
