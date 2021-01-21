using UnityEngine;

public class MobileTargetFPS : MonoBehaviour
{
    #if UNITY_ANDROID || UNITY_IOS
        void Awake() {
            Application.targetFrameRate = 60;
        }
    #endif
}
