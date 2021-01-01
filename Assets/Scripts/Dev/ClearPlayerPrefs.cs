using UnityEngine;

public class ClearPlayerPrefs : MonoBehaviour
{
    [ContextMenu("Clear PlayerPrefs")]
    void clearPlayerPrefs() {
        PlayerPrefs.DeleteAll();
    }
}