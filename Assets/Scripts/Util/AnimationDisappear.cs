using UnityEngine;

/// <summary>
/// Util class for objects to automatically disable in an Animation.
/// </summary>
public class AnimationDisappear : MonoBehaviour
{
    void Run() {
        gameObject.SetActive(false);
    }
}
