using UnityEngine;

/// <summary>
/// Handles player collision with objects, like obstacles or power-ups.
/// </summary>
public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private EndGame gameEndManager;
    [SerializeField] private PowerUpSpawner powerUpSpawner;
    private PlayerPowerUp playerPowerUp;

    void Start() {
        playerPowerUp = GetComponent<PlayerPowerUp>();
    }

    private void OnTriggerEnter(Collider other) {
        GameObject otherObject = other.gameObject;
        if (otherObject != gameObject) {
            if (otherObject.CompareTag(TagHolder.OBSTACLE_TAG)) {
                if (otherObject.transform.position.x > 0) {
                    collideWithObstacle(other);
                }
            }
            else if (otherObject.CompareTag(TagHolder.POWERUP_TAG)) {
                PowerUp powerUpObject;
                if (otherObject.TryGetComponent<PowerUp>(out powerUpObject)) {
                    Row row;
                    if (powerUpObject.transform.parent.TryGetComponent<Row>(out row)) {
                        powerUpSpawner.DespawnPowerUp(row);
                        playerPowerUp.AddPowerUp(powerUpObject.Type);
                    }
                }
            }
        }
    }

    private void collideWithObstacle(Collider other) {
        //HARDENED POWER-UP OVERRIDE OBSTACLE HITTING
        if (playerPowerUp.GetActivePowerUp() == PowerUpType.Hardened) {
            other.enabled = false;

            playerPowerUp.RemovePowerUp();
            return;
        }

        gameEndManager.endGame();
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
    }
    
}
