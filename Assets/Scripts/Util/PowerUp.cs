using UnityEngine;

public class PowerUp : MonoBehaviour {
    [SerializeField] private PowerUpType type;
    public PowerUpType Type => type;
}