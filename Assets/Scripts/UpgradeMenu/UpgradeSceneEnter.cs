using System.Collections;
using UnityEngine;

public class UpgradeSceneEnter : MonoBehaviour
{
    void Start() {
        if (PlayerPrefs.GetInt(TagHolder.PREF_SKIP_ANIM) == 1) return;

        Animator mainCamAnim = Camera.main.GetComponent<Animator>();
        mainCamAnim.Play(TagHolder.CAM_UPGRADES_ENTER);
    }
}
