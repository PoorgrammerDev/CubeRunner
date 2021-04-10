using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject baseStoreScreen; 
    [SerializeField] private BitsDisplay bitsDisplay;

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
    private SkillViewData activeSVData;

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
    [SerializeField] private Image buyBitIcon0; 
    [SerializeField] private Image buyBitIcon1; 
    [SerializeField] private Button buyButton; 
    private ActiveBuyData? activeBuyData; 

    [Header("Other References")]
    [SerializeField] private SaveManager saveManager;


    [Header("Input Data")]
    [SerializeField] private Sprite[] powerUpSprites;

    [SerializeField] private Color levelIndicatorHighlight;
    [SerializeField] private Color bitsColor;

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

                //set active data
                activeSVData = menuData;

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
                SetLevelDots(leftLevelDots, saveManager.GetUpgradeLevel(menuData.powerUpType, 0), -1);
                SetLevelDots(rightLevelDots, saveManager.GetUpgradeLevel(menuData.powerUpType, 1), -1);

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
        int currentLevel = saveManager.GetUpgradeLevel(menuData.PowerUpType, menuData.PathIndex);
        if (currentLevel < 4) currentLevel++;

        return OpenBuyScreen(menuData, currentLevel);
    }

    public bool OpenBuyScreen (BuyMenuData menuData, int level) {
        if (!buyScreen.activeInHierarchy) {
            int currentLevel = saveManager.GetUpgradeLevel(menuData.PowerUpType, menuData.PathIndex);

            BuyMenuEntry dataEntry;
            try {
                dataEntry = menuData.GetDataEntry(level);
            }
            catch (System.Exception) {
                Debug.LogError("Buy Menu Index out of bounds");
                return false;
            }

            //set active data
            activeBuyData = new ActiveBuyData(menuData, level);

            //change names
            buyScreenPUPName.text = menuData.PowerUpType.ToString();
            pathName.text = (menuData.PathName != null && menuData.PathName.Length > 0) ? menuData.PathName : "";

            //change description
            buyDescription.text = menuData.Description;

            //change path sprite
            upgradePathIcon.sprite = menuData.PathSymbol;
            
            //change level dots
            SetLevelDots(buyLevelDots, Mathf.Min(currentLevel + 1, 4), level);

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

            //check if upgrade is not already owned
            if (level <= currentLevel) {
                SetBuyButtonActive(false, false);
                buyCost.text = "OWNED";
            }
            //check if upgrade is not locked
            else if (level > currentLevel + 1) {
                SetBuyButtonActive(false, false);
                buyCost.text = "LOCKED";
            }
            //check if upgrade can be afforded
            else if (dataEntry.cost <= saveManager.TotalBits) {
                SetBuyButtonActive(true, true);
            }
            //if if can't be afforded
            else {
                SetBuyButtonActive(false, true);
            }

            //if skillview is skipped, then clear sv data (so when returning, it returns to base store)
            if (baseStoreScreen.activeInHierarchy) {
                activeSVData = null;
            }

            //close all other submenus and activate this menu
            baseStoreScreen.SetActive(false);
            skillViewScreen.SetActive(false);
            buyScreen.SetActive(true);
            return true;
        }
        return false;
    }

    private void SetBuyButtonActive (bool active, bool bitsIconActive) {
        buyButton.interactable = active;
        buyCost.color = active ? bitsColor : Color.gray;
        buyBitIcon0.gameObject.SetActive(bitsIconActive);
        buyBitIcon1.gameObject.SetActive(bitsIconActive);

        if (bitsIconActive) {
            buyBitIcon0.color = active ? bitsColor : Color.gray;
            buyBitIcon1.color = active ? bitsColor : Color.gray;
        }
    }

    public void BuyUpgrade() {
        //check buy screen active and data is present
        if (buyScreen.activeInHierarchy && activeBuyData != null) {
            BuyMenuData menuData = activeBuyData.Value.menuData;
            int level = activeBuyData.Value.level;

            //check if the level trying to upgrade to is correct (is the next level)
            if (saveManager.GetUpgradeLevel(menuData.PowerUpType, menuData.PathIndex) + 1 == level) {
                //subtract cost (also acts as a check for if affordable)
                if (saveManager.SubtractBits(menuData.GetDataEntry(level).cost)) {
                    //upgrades
                    saveManager.SetUpgradeLevel(menuData.PowerUpType, menuData.PathIndex, level);

                    //TODO: sfx
                    
                    //TODO: cube animation

                    bitsDisplay.UpdateDisplay();

                    //exits menu
                    BuyMenuReturn();
                }
            }
        }
    }

    public void BuyMenuReturn() {
        if (buyScreen.activeInHierarchy) {
            if (activeSVData != null) {
                OpenSkillView(activeSVData);
            }
            else {
                OpenBaseStore();
            }
        }
    }

    private bool SetLevelDots(Image[] dots, int fillLevel, int highlightLevel) {
        if (fillLevel >= 0 && fillLevel <= dots.Length) {
            for (int i = 0; i < fillLevel; i++) {
                dots[i].color = Color.white;
            }
            for (int i = fillLevel; i < dots.Length; i++) {
                dots[i].color = Color.gray;
            }

            if (highlightLevel != -1) {
                dots[--highlightLevel].color = levelIndicatorHighlight;
            }
            return true;
        }
        return false;
    }

    public void ExitShop() {
        SceneManager.LoadScene(TagHolder.MAIN_MENU_SCENE, LoadSceneMode.Single);
    }

    [System.Serializable]
    private struct ActiveBuyData {
        public BuyMenuData menuData;
        public int level;

        public ActiveBuyData(BuyMenuData menuData, int level) {
            this.menuData = menuData;
            this.level = level;
        }
    }

}