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
    [SerializeField] private Image[] leftPath; 
    [SerializeField] private BuyMenuDataHolder rightBMDHolder; 
    [SerializeField] private TextMeshProUGUI SVRightField; 
    [SerializeField] private Image rightPathIcon; 
    [SerializeField] private Image[] rightLevelDots; 
    [SerializeField] private Image[] rightPath; 
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
    [SerializeField] private Animator cubeAnimator;
    [SerializeField] private MusicManager musicManager;

    [Header("Input Data")]
    [SerializeField] private Sprite[] powerUpSprites;

    [SerializeField] private Color levelIndicatorHighlight;
    [SerializeField] private Color bitsColor;
    [SerializeField] private Color refundColor;
    [SerializeField] private float refundPenaltyPercent;

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

                //set active path
                UpdatePathDisplay(menuData.powerUpType, saveManager.GetActivePath(menuData.powerUpType) == 1);

                //close all other submenus and activate this menu
                buyScreen.SetActive(false);
                baseStoreScreen.SetActive(false);
                skillViewScreen.SetActive(true);
                return true;
            }
        }
        return false;
    }

    //TODO: this function and this entire L/R system is pretty messy, cleanup is needed
    public void ChangeActivePath(bool right) {
        if (!skillViewScreen.activeInHierarchy || activeSVData == null) return;
        if (saveManager.GetUpgradeLevel(activeSVData.powerUpType, right ? 1 : 0) <= 0) return;

        saveManager.SetActivePath(activeSVData.powerUpType, right ? 1 : 0);
        UpdatePathDisplay(activeSVData.powerUpType, right);
    }

    //TODO: this function and this entire L/R system is pretty messy, cleanup is needed
    private void UpdatePathDisplay(PowerUpType powerUpType, bool right) {
        Color leftColor = Color.white;
        Color rightColor = Color.white;
        if (saveManager.GetUpgradeLevel(powerUpType, right ? 1 : 0) > 0) {
            leftColor = right ? Color.white : levelIndicatorHighlight;
            rightColor = right ? levelIndicatorHighlight : Color.white;
        }

        SVLeftField.color = leftColor;
        SVRightField.color = rightColor;

        foreach (Image pathObject in leftPath) {
            pathObject.color = leftColor;
        }

        foreach (Image pathObject in rightPath) {
            pathObject.color = rightColor;
        }

        foreach (Image levelDot in leftLevelDots) {
            if (!levelDot.color.CompareRGB(Color.gray)) {
                levelDot.color = leftColor;
            }
        }

        foreach (Image levelDot in rightLevelDots) {
            if (!levelDot.color.CompareRGB(Color.gray)) {
                levelDot.color = rightColor;
            }
        }
    }

    public void OpenBuyScreen (BuyMenuData menuData) {
        int currentLevel = saveManager.GetUpgradeLevel(menuData.PowerUpType, menuData.PathIndex);
        if (currentLevel < 4) currentLevel++;

        OpenBuyScreen(menuData, currentLevel);
    }

    public void OpenBuyScreen (int level) {
        if (activeBuyData != null && activeBuyData.HasValue && activeBuyData.Value.menuData != null) {
            OpenBuyScreen(activeBuyData.Value.menuData, level);
        }
    }

    public void OpenBuyScreen (BuyMenuData menuData, int level) {
        int currentLevel = saveManager.GetUpgradeLevel(menuData.PowerUpType, menuData.PathIndex);

        BuyMenuEntry dataEntry;
        try {
            dataEntry = menuData.GetDataEntry(level);
        }
        catch (System.Exception) {
            Debug.LogError("Buy Menu Index out of bounds");
            return;
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
        SetLevelDots(buyLevelDots, currentLevel, level);

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

        //if upgrade is already owned and is less than current level, it cannot be refunded
        if (level < currentLevel) {
            SetBuyButtonActive(false, false, Color.gray);
            buyCost.text = "OWNED";
        }
        //if upgrade is owned and is the current level, it can be refunded
        else if (level == currentLevel) {
            SetBuyButtonActive(true, false, refundColor);
            buyCost.text = "REFUND   " + ((int) (dataEntry.cost * (1f - refundPenaltyPercent))).ToString();
        }
        //if upgrade is higher than the next level, it is locked
        else if (level > currentLevel + 1) {
            SetBuyButtonActive(false, false, Color.gray);
            buyCost.text = "LOCKED";
        }
        //otherwise, continue buying process as normal
        //check if upgrade can be afforded
        else if (dataEntry.cost <= saveManager.TotalBits) {
            SetBuyButtonActive(true, true, bitsColor);
            buyCost.text = dataEntry.cost.ToString();
        }
        //if it can't be afforded
        else {
            SetBuyButtonActive(false, true, Color.gray);
            buyCost.text = dataEntry.cost.ToString();
        }

        //if skillview is skipped, then clear sv data (so when returning, it returns to base store)
        if (baseStoreScreen.activeInHierarchy) {
            activeSVData = null;
        }

        //close all other submenus and activate this menu
        baseStoreScreen.SetActive(false);
        skillViewScreen.SetActive(false);
        buyScreen.SetActive(true);
    }

    private void SetBuyButtonActive (bool active, bool bitsIconActive, Color color) {
        if (active) {
            ColorBlock buyButtonColorBlock = buyButton.colors;
            buyButtonColorBlock.normalColor = color;
            buyButtonColorBlock.highlightedColor = color * 1.25f;
            buyButtonColorBlock.pressedColor = Color.Lerp(color, Color.black, 0.2f); 
            buyButtonColorBlock.selectedColor = buyButtonColorBlock.highlightedColor;
            buyButton.colors = buyButtonColorBlock;
        }
        buyButton.interactable = active;
        
        buyCost.color = color;
        buyBitIcon0.gameObject.SetActive(bitsIconActive);
        buyBitIcon1.gameObject.SetActive(bitsIconActive);

        if (bitsIconActive) {
            buyBitIcon0.color = color;
            buyBitIcon1.color = color;
        }
    }

    public void PressBuyButton() {
        //check buy screen active and data is present
        if (!buyScreen.activeInHierarchy || activeBuyData == null) return;

        int level = activeBuyData.Value.level;
        int currentLevel = saveManager.GetUpgradeLevel(activeBuyData.Value.menuData.PowerUpType, activeBuyData.Value.menuData.PathIndex);

        //current level - REFUND
        if (level == currentLevel) {
            RefundUpgrade();
        }
        //next level - BUY
        else if (level == (currentLevel + 1)) {
            BuyUpgrade();
        }
    }

    private void BuyUpgrade() {
        BuyMenuData menuData = activeBuyData.Value.menuData;
        int level = activeBuyData.Value.level;
        //check if the level trying to upgrade to is correct (is the next level)
        if (saveManager.GetUpgradeLevel(menuData.PowerUpType, menuData.PathIndex) + 1 == level) {
            //subtract cost (also acts as a check for if affordable)
            if (saveManager.SubtractBits(menuData.GetDataEntry(level).cost)) {
                //upgrades
                saveManager.SetUpgradeLevel(menuData.PowerUpType, menuData.PathIndex, level);
                
                //effects and update display (particles and sfx are handled by animation)
                cubeAnimator.SetTrigger(TagHolder.UPG_CUBE_TRIGGER);
                StartCoroutine(bitsDisplay.TransitionDisplay(2.5f));

                //exits menu
                BuyMenuReturn();
                
                //If the other path does not have any upgrades, set the active path as this one.
                //TODO: This code isn't particularly clean (neither is the entire left/right upg system)
                if (menuData.PathIndex == 1 && saveManager.GetUpgradeLevel(menuData.PowerUpType, 0) == 0) {
                    ChangeActivePath(true);
                }
                else if (menuData.PathIndex == 0 && saveManager.GetUpgradeLevel(menuData.PowerUpType, 1) == 0) {
                    ChangeActivePath(false);
                }
            }
        }
    }

    private void RefundUpgrade() {
        BuyMenuData menuData = activeBuyData.Value.menuData;
        int level = activeBuyData.Value.level;

        if (saveManager.GetUpgradeLevel(menuData.PowerUpType, menuData.PathIndex) == level) {
            int refundAmount = (int) (menuData.GetDataEntry(level).cost * (1f - refundPenaltyPercent));
            saveManager.AddBits(refundAmount);

            saveManager.SetUpgradeLevel(menuData.PowerUpType, menuData.PathIndex, level - 1);
            StartCoroutine(bitsDisplay.TransitionDisplay(2.5f));

            BuyMenuReturn();
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

    //TODO: This scene management stuff should be separated into a different script
    public void ExitShop() {
        if (PlayerPrefs.GetInt(TagHolder.PREF_SKIP_ANIM) == 1) {
            ChangeSceneToMM();
        }
        else {
            Animator mainCamAnim = Camera.main.GetComponent<Animator>();
            mainCamAnim.Play(TagHolder.CAM_UPGRADES_EXIT);

            StartCoroutine(musicManager.FadeOutAndStop(1f));
        }
    }

    public void ChangeSceneToMM() {
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