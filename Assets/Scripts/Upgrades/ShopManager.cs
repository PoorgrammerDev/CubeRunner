using System.Collections.Generic;
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
    [SerializeField] private Image leftLevelIcon; 
    [SerializeField] private BuyMenuDataHolder rightBMDHolder; 
    [SerializeField] private TextMeshProUGUI SVRightField; 
    [SerializeField] private Image rightLevelIcon; 
    [SerializeField] private TextMeshProUGUI PUPName; 

    [Header("UI References - Buy Screen")]
    [SerializeField] private GameObject buyScreen; 
    [SerializeField] private Image buyLevelIcon; 
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
    [SerializeField] private Sprite[] levelSprites;



    /**************************
    SCREEN NAVIGATION FUNCTIONS
    **************************/

    public void OpenBaseStore () {
        skillViewScreen.SetActive(false);
        buyScreen.SetActive(false);

        baseStoreScreen.SetActive(true);
    }

    public bool OpenSkillView (SkillViewData skillViewData) {
        if (!skillViewScreen.activeInHierarchy) {
            //change central sprite to correct power-up sprite 
            if ((int) skillViewData.powerUpType >= 0 && (int) skillViewData.powerUpType < powerUpSprites.Length) {
                powerUpIconDisp.sprite = powerUpSprites[(int) skillViewData.powerUpType];

                // NOTE: -----------------------------------
                // A check for if RIGHT exists isn't needed
                // because if RIGHT doesn't exist, this menu
                // is skipped entirely.
                // -----------------------------------------

                //load in BMD
                leftBMDHolder.data = skillViewData.leftBuyMenuData;
                rightBMDHolder.data = skillViewData.rightBuyMenuData;

                //set names
                SVLeftField.text = skillViewData.leftFieldName;
                SVRightField.text = skillViewData.rightFieldName;
                PUPName.text = skillViewData.powerUpType.ToString();

                //set sprites to correct levels
                leftLevelIcon.sprite = levelSprites[saveManager.GetUpgradeLevel(skillViewData.powerUpType, 0)];
                rightLevelIcon.sprite = levelSprites[saveManager.GetUpgradeLevel(skillViewData.powerUpType, 1)];

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
            int nextLevel = saveManager.GetUpgradeLevel(menuData.PowerUpType, menuData.PathIndex) + 1;
            BuyMenuEntry dataEntry;
            try {
                dataEntry = menuData.GetDataEntry(nextLevel);
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

            //change level sprite
            buyLevelIcon.sprite = levelSprites[nextLevel];

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

}


