using UnityEngine;

public class MobileLaunchSettings : MonoBehaviour
{
    [SerializeField] private MobileDetector mobileDetector;
    
    #if UNITY_ANDROID || UNITY_IOS
        void Awake() {
            Application.targetFrameRate = 60;
        }
    #endif

    #if UNITY_WEBGL
        void Awake() {
            if (mobileDetector.isMobile()) {
                Application.targetFrameRate = 60;
            }
        }
    #endif
}
