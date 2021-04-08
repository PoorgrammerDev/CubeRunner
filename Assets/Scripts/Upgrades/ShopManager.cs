using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject baseStoreScreen; 

    [Header("UI References - Skill View")]
    [SerializeField] private GameObject skillViewScreen; 
    [SerializeField] private Image powerUpIconDisp; 
    [SerializeField] private BuyMenuDataHolder leftBMDHolder; 
    [SerializeField] private TextMeshProUGUI SVLeftField; 
    [SerializeField] private Image leftPathIcon; 
    [SerializeField] private Image[] leftLevelDots; 
    [SerializeField] private BuyMenuDataHolder rightBMDHolder; 
    [SerializeField] private TextMeshProUGUI SVRightField; 
    [SerializeField] private Image rightPathIcon; 
    [SerializeField] private Image[] rightLevelDots; 
    [SerializeField] private TextMeshProUGUI PUPName; 

    [Header("UI References - Buy Screen")]
    [SerializeField] private GameObject buyScreen; 
    [SerializeField] private Image upgradePathIcon; 
    [SerializeField] private Image[] buyLevelDots; 
    [SerializeField] private TextMeshProUGUI buyScreenPUPName; 
    [SerializeField] private TextMeshProUGUI pathName; 
    [SerializeField] private TextMeshProUGUI buyDescription; 
    [SerializeField] private TextMeshProUGUI statName0; 
    [SerializeField] private TextMeshProUGUI statName1; 
    [SerializeField] private TextMeshProUGUI statChange0; 
    [SerializeField] private TextMeshProUGUI statChange1; 
    [SerializeField] private TextMeshProUGUI buyCost; 
    private bool skillViewBypassed = true;

    [Header("Other References")]
    [SerializeField] private SaveManager saveManager;


    [Header("Input Data")]
    [SerializeField] private Sprite[] powerUpSprites;

    [SerializeField] private Color levelIndicatorHighlight;

    /**************************
    SCREEN NAVIGATION FUNCTIONS
    **************************/

    public void OpenBaseStore () {
        skillViewScreen.SetActive(false);
        buyScreen.SetActive(false);

        baseStoreScreen.SetActive(true);
    }

    public bool OpenSkillView (SkillViewData menuData) {
        if (!skillViewScreen.activeInHierarchy) {
            //change central sprite to correct power-up sprite 
            if ((int) menuData.powerUpType >= 0 && (int) menuData.powerUpType < powerUpSprites.Length) {
                powerUpIconDisp.sprite = powerUpSprites[(int) menuData.powerUpType];

                // NOTE: -----------------------------------
                // A check for if RIGHT exists isn't needed
                // because if RIGHT doesn't exist, this menu
                // is skipped entirely.
                // -----------------------------------------

                //load in BMD
                leftBMDHolder.data = menuData.leftBuyMenuData;
                rightBMDHolder.data = menuData.rightBuyMenuData;

                //set names
                SVLeftField.text = menuData.leftFieldName;
                SVRightField.text = menuData.rightFieldName;
                PUPName.text = menuData.powerUpType.ToString();

                //set sprites to correct path symbols
                leftPathIcon.sprite = menuData.leftSymbol;
                rightPathIcon.sprite = menuData.rightSymbol;


                //set level dots
                SetLevelDots(leftLevelDots, saveManager.GetUpgradeLevel(menuData.powerUpType, 0), false);
                SetLevelDots(rightLevelDots, saveManager.GetUpgradeLevel(menuData.powerUpType, 1), false);

                //close all other submenus and activate this menu
                buyScreen.SetActive(false);
                baseStoreScreen.SetActive(false);
                skillViewScreen.SetActive(true);
                return true;
            }
        }
        return false;
    }


    public bool OpenBuyScreen (BuyMenuData menuData) {
        if (!buyScreen.activeInHierarchy) {
            int level = saveManager.GetUpgradeLevel(menuData.PowerUpType, menuData.PathIndex) + 1;
            BuyMenuEntry dataEntry;
            try {
                dataEntry = menuData.GetDataEntry(level);
            }
            catch (System.Exception) {
                Debug.LogError("Buy Menu Index out of bounds");
                return false;
            }

            //change names
            buyScreenPUPName.text = menuData.PowerUpType.ToString();
            pathName.text = (menuData.PathName != null && menuData.PathName.Length > 0) ? menuData.PathName : "";

            //change description
            buyDescription.text = menuData.Description;

            //change path sprite
            upgradePathIcon.sprite = menuData.PathSymbol;
            
            //change level dots
            SetLevelDots(buyLevelDots, level, true);

            //fill in first stat
            statName0.text = dataEntry.statName0;
            statChange0.text = dataEntry.statChange0;
            
            //if present, fill in second stat
            if (dataEntry.statName1 != null && dataEntry.statName1.Length > 0) {
                statName1.gameObject.SetActive(true);
                statChange1.gameObject.SetActive(true);

                statName1.text = dataEntry.statName1;
                statChange1.text = dataEntry.statChange1;
            }
            else {
                statName1.gameObject.SetActive(false);
                statChange1.gameObject.SetActive(false);
            }

            //set buy price
            buyCost.text = dataEntry.cost.ToString();

            skillViewBypassed = baseStoreScreen.activeInHierarchy;

            //close all other submenus and activate this menu
            baseStoreScreen.SetActive(false);
            skillViewScreen.SetActive(false);
            buyScreen.SetActive(true);
            return true;
        }
        return false;
    }

    public void BuyMenuReturn() {
        if (buyScreen.activeInHierarchy) {
            if (skillViewBypassed) {
                OpenBaseStore();
            }
            else {
                buyScreen.SetActive(false);
                baseStoreScreen.SetActive(false);
                skillViewScreen.SetActive(true);
            }
        }
    }

    private bool SetLevelDots(Image[] dots, int level, bool highlightCurrent) {
        if (level >= 0 && level <= dots.Length) {
            for (int i = 0; i < level; i++) {
                dots[i].color = Color.white;
            }
            for (int i = level; i < dots.Length; i++) {
                dots[i].color = Color.gray;
            }

            if (highlightCurrent) dots[--level].color = levelIndicatorHighlight;
            return true;
        }
        return false;
    }

}