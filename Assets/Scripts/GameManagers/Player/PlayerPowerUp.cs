using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPowerUp : MonoBehaviour
{
    [SerializeField] private bool hasPowerUp = false;
    public bool HasPowerUp => hasPowerUp;
    [SerializeField] private PowerUpType powerUpType;
    
    public void AddPowerUp (PowerUpType type) {
        powerUpType = type;
        hasPowerUp = true;

        if (type == PowerUpType.Hardened) {
            hardened.SetActive(true);
        }
    }

    public void RemovePowerUp() {
        if (hasPowerUp) {
            hasPowerUp = false;

            if (powerUpType == PowerUpType.Hardened) {
                hardened.SetActive(false);
            }
        }
    }

    public PowerUpType? GetActivePowerUp () {
        if (hasPowerUp) {
            return powerUpType;
        }
        return null;
    }

    [SerializeField] private GameObject hardened;
}
