using UnityEngine;

public class SkillViewDataHolder : MonoBehaviour {
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private SkillViewData data;

    public void Click() {
        shopManager.OpenSkillView(data);
    }
}
