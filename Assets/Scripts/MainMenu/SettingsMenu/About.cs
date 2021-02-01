using UnityEngine;

public class About : MonoBehaviour
{
    public void OpenInstagram() {
        Application.OpenURL(TagHolder.LINK_INSTAGRAM);
    }
}
