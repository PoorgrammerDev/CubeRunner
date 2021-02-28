using UnityEngine;
using UnityEngine.UI;

public class ButtonAutoSelect : MonoBehaviour
{
    void OnEnable() {
        GetComponent<Button>().Select();
    }
}
