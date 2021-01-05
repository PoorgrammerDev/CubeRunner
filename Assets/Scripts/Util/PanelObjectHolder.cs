using UnityEngine;

/// <summary>
/// Util class to hold Screen Objects, for easy transition during Game Start. 
/// </summary>
public class PanelObjectHolder : MonoBehaviour
{
    [SerializeField] private Transform[] leftScreenObjects, rightScreenObjects, topScreenObjects, bottomScreenObjects;
    public Transform[] LeftScreenObjects { get => leftScreenObjects; set => leftScreenObjects = value; }
    public Transform[] RightScreenObjects { get => rightScreenObjects; set => rightScreenObjects = value; }
    public Transform[] TopScreenObjects { get => topScreenObjects; set => topScreenObjects = value; }
    public Transform[] BottomScreenObjects { get => bottomScreenObjects; set => bottomScreenObjects = value; }

    public void DeactivateAll() {
        foreach (Transform leftScreenObject in leftScreenObjects) {
            leftScreenObject.gameObject.SetActive(false);
        }
        foreach (Transform rightScreenObject in rightScreenObjects) {
            rightScreenObject.gameObject.SetActive(false);
        }
        foreach (Transform topScreenObject in topScreenObjects) {
            topScreenObject.gameObject.SetActive(false);
        }
        foreach (Transform bottomScreenObject in bottomScreenObjects) {
            bottomScreenObject.gameObject.SetActive(false);
        }
    }

}
