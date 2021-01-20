using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class About : MonoBehaviour
{
    public void OpenFeedback() {
        Application.OpenURL(TagHolder.LINK_FEEDBACK);
    }

    public void OpenLinkedIn() {
        Application.OpenURL(TagHolder.LINK_LINKEDIN);
    }

    public void OpenWebsite() {
        Application.OpenURL(TagHolder.LINK_WEBSITE);    
    }

    public void OpenInstagram() {
        Application.OpenURL(TagHolder.LINK_INSTAGRAM);
    }
}
