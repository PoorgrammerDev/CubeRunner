using UnityEngine;
using TMPro;

public class Credits : MonoBehaviour
{
    [SerializeField] private TextAsset textAsset;
    private TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start() {
        text = GetComponent<TextMeshProUGUI>();
        text.text = textAsset.text;
    }
}