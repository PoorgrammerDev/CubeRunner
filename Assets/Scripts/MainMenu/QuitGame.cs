using UnityEngine;

public class QuitGame : MonoBehaviour
{

    // Start is called before the first frame update
    void Start() {
        #if UNITY_WEBGL || UNITY_ANDROID || UNITY_IOS
            gameObject.SetActive(false);
        #endif
    }

    public void Quit() {
        Application.Quit();
    }
}