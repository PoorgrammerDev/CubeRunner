using UnityEngine;

/// <summary>
/// Util class to hold Screen Objects, for easy transition during Game Start. 
/// </summary>
public class PanelObjectHolder : MonoBehaviour
{
    [SerializeField] private GameObject leftScreenObjects, rightScreenObjects, topScreenObjects, bottomScreenObjects;

    public void MoveOut() {
        leftScreenObjects.GetComponent<Animator>().Play(TagHolder.UI_ANIM_EXIT_LEFT);
        rightScreenObjects.GetComponent<Animator>().Play(TagHolder.UI_ANIM_EXIT_RIGHT);
        topScreenObjects.GetComponent<Animator>().Play(TagHolder.UI_ANIM_EXIT_UP);
        bottomScreenObjects.GetComponent<Animator>().Play(TagHolder.UI_ANIM_EXIT_DOWN);
    }
}
