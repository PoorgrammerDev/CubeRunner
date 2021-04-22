using UnityEngine;

[CreateAssetMenu(fileName = "New Skill View", menuName = "Upgrade Menu/Skill View")]
public class SkillViewData : ScriptableObject
{
    public PowerUpType powerUpType;

    public string leftFieldName;
    public BuyMenuData leftBuyMenuData;
    public Sprite leftSymbol;

    public string rightFieldName;
    public BuyMenuData rightBuyMenuData;
    public Sprite rightSymbol;
}