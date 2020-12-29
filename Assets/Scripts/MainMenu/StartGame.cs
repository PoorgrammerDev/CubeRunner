using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{

    [SerializeField]
    private Material glowingMaterial;

    [SerializeField]
    private Transform[] rightScreenObjects;

    [SerializeField]
    private Transform[] bottomScreenObjects;

    private MeshRenderer renderer;

    private Vector3 moveRight = new Vector3(0.05f, 0, 0);
    private Vector3 moveRightUI = new Vector3(9f, 0, 0);

    private Vector3 moveDown = new Vector3(0, -0.05f, 0);
    private Vector3 moveDownUI = new Vector3(0, -3f, 0);

    private const int UI_LAYER = 5;

    void Start() {
        renderer = gameObject.GetComponent<MeshRenderer>();
    }

    public void Click() {
        renderer.material = glowingMaterial;
        StartCoroutine("RemoveOtherObjects");
    }

    IEnumerator RemoveOtherObjects () {
        float minRight = float.MaxValue;
        float maxBottom = float.MinValue;
        
        do {
            foreach (Transform rightScreenObject in rightScreenObjects) {
                rightScreenObject.Translate((rightScreenObject.gameObject.layer == UI_LAYER) ? moveRightUI : moveRight, Space.World);

                if (rightScreenObject.position.x < minRight) {
                    minRight = rightScreenObject.position.x;
                }
            }

            foreach (Transform bottomScreenObject in bottomScreenObjects) {
                bottomScreenObject.Translate((bottomScreenObject.gameObject.layer == UI_LAYER) ? moveDownUI : moveDown, Space.World);

                if (bottomScreenObject.position.y > maxBottom) {
                    maxBottom = bottomScreenObject.position.y;
                }
            }
            yield return null;
        } while (minRight < 2000 && maxBottom > -2000);

        yield break;
    }
}
