using UnityEngine;

public class MobileLaunchSettings : MonoBehaviour
{
    #if UNITY_ANDROID || UNITY_IOS
        void Awake() {
            Application.targetFrameRate = 60;
            //Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
    #endif
}
