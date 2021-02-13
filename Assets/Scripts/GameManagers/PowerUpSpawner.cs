using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is used for spawning in Power Ups.
/// </summary>
public class PowerUpSpawner : MonoBehaviour
{

    [SerializeField] private GameValues gameValues;
    [SerializeField] private int cooldown;
    [SerializeField] private PowerUp[] powerUpObjects;
    [SerializeField] private Animator PUPTypeDisplay;
    [SerializeField] private Image TypeDisplayImage;
    [SerializeField] private Sprite[] sprites;
    
    private int cooldownTracker = 0;

    public bool SpawnPowerUp(Row row, int slot, int lanes) {
        return SpawnPowerUp(row, slot, lanes, (PowerUpType) Random.Range(0, (int) PowerUpType.COUNT));
    }

    public bool SpawnPowerUp (Row row, int slot, int lanes, PowerUpType powerUpType) {
        if (IsReady()) {
            bool[] rowStructs = row.structures;
            if (rowStructs[slot]) { //checks that the position indicated is a gap
                PowerUp powerUpObject = powerUpObjects[(int) powerUpType];

                //re-parent
                row.powerUp = powerUpObject;
                powerUpObject.transform.SetParent(row.transform, false);

                //change position
                Vector3 position = powerUpObject.transform.localPosition;
                position.z = (((lanes) / 2f) - slot - 0.5f) * gameValues.WidthScale;
                powerUpObject.transform.localPosition = position;

                //colorblind indicator
                if (PlayerPrefs.GetInt(TagHolder.PREF_COLORBLIND_MODE) == 1) {
                    TypeDisplayImage.sprite = sprites[(int) powerUpType];
                    
                    PUPTypeDisplay.gameObject.SetActive(true);
                    PUPTypeDisplay.ResetTrigger(TagHolder.HUD_EXIT_TRIGGER);
                    StartCoroutine(RemoveTypeGUI());
                }
                
                powerUpObject.gameObject.SetActive(true);

                //set cooldown
                cooldownTracker = cooldown;
                return true;
            }
        }
        return false;
    }

    public bool DespawnPowerUp (Row row) {
        if (row.HasPowerUp()) {
            PowerUp powerUp = row.powerUp;
            powerUp.transform.parent = transform;
            powerUp.gameObject.SetActive(false);
            row.powerUp = null;
            return true;
        }
        return false;
    }

    public bool IsReady() {
        return (cooldownTracker == 0 && !IsDeployed());
    }

    public bool IsDeployed() {
        foreach (PowerUp powerUpObject in powerUpObjects) {
            if (powerUpObject.gameObject.activeInHierarchy) {
                return true;
            }
        }
        return false;
    }

    public void TickCooldown() {
        if (cooldownTracker > 0) cooldownTracker--;
    }

    IEnumerator RemoveTypeGUI() {
        do {
            yield return null;
        } while (gameValues.GameActive && IsDeployed());

        PUPTypeDisplay.SetTrigger(TagHolder.HUD_EXIT_TRIGGER);
    }
}
