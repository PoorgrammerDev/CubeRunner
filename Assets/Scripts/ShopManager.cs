using System.Collections;
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
    [SerializeField] private TextMeshProUGUI SVLeftField; 
    [SerializeField] private TextMeshProUGUI SVRightField; 

    [Header("UI References - Buy Screen")]
    [SerializeField] private GameObject buyScreen; 
    [SerializeField] private Image buyLevelIcon; 
    [SerializeField] private TextMeshProUGUI buyDescription; 
    [SerializeField] private TextMeshProUGUI statName0; 
    [SerializeField] private TextMeshProUGUI statName1; 
    [SerializeField] private TextMeshProUGUI statChange0; 
    [SerializeField] private TextMeshProUGUI statChange1; 
    [SerializeField] private TextMeshProUGUI buyCost; 

    [Header("Other References")]
    [SerializeField] private SaveManager saveManager;


    [Header("Input Data")]
    [SerializeField] private Dictionary<PowerUpType, Sprite> powerUpSprites;
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
            Sprite tempSprite;
            if (powerUpSprites.TryGetValue(skillViewData.powerUpType, out tempSprite)) {
                powerUpIconDisp.sprite = tempSprite;

                //change descriptions to suit
                SVLeftField.text = skillViewData.leftFieldName;
                SVRightField.text = skillViewData.rightFieldName;


                //TODO: get existing data from SaveManager to find what levels the two tiers are


                //close all other submenus and activate this menu
                buyScreen.SetActive(false);
                baseStoreScreen.SetActive(false);
                skillViewScreen.SetActive(true);
                return true;
            }
        }
        return false;
    }


    public bool OpenBuyScreen (BuyMenuDataHolder dataHolder) {
        if (!buyScreen.activeInHierarchy) {
            BuyMenuData buyMenuData;
            try {
                buyMenuData = dataHolder.GetBuyMenuData(0); //TODO: GET PROPER LEVEL LATER
            }
            catch (System.Exception) {
                Debug.LogError("Buy Menu Index out of bounds");
                return false;
            }

            //change description
            buyDescription.text = dataHolder.Description;

            //TODO: change level sprite based on level

            //fill in first stat
            statName0.text = buyMenuData.statName0;
            statChange0.text = buyMenuData.statChange0;
            
            //if present, fill in second stat
            if (buyMenuData.statName1 != null && buyMenuData.statName1.Length > 0) {
                statName1.gameObject.SetActive(true);
                statChange1.gameObject.SetActive(true);

                statName1.text = buyMenuData.statName1;
                statChange1.text = buyMenuData.statChange1;
            }
            else {
                statName1.gameObject.SetActive(false);
                statChange1.gameObject.SetActive(false);
            }

            //set buy price
            buyCost.text = buyMenuData.cost.ToString();

            return true;
        }
        return false;
    }




}


