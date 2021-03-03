using UnityEngine;

/// <summary>
/// This class represents the actual physical Power-up object that the player picks up
/// </summary>
public class PowerUp : MonoBehaviour {
    [SerializeField] private PowerUpType type;
    public PowerUpType Type => type;

    [SerializeField] private AbstractPowerUp pupMechanism;
    public AbstractPowerUp PUPMechanism => pupMechanism;
}