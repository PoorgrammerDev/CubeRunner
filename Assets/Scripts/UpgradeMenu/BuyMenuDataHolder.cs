using UnityEngine;

public class BuyMenuDataHolder : MonoBehaviour {
    [SerializeField] private ShopManager shopManager;
    public BuyMenuData data;

    public void Click() {
        if (data != null) shopManager.OpenBuyScreen(data);
    }
}