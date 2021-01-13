using UnityEngine;
using System.Collections;

/// <summary>
/// Handles player collision with objects, like obstacles or power-ups.
/// </summary>
public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private GameValues gameValues;
    [SerializeField] private EndGame gameEndManager;
    [SerializeField] private PowerUpSpawner powerUpSpawner;
    [SerializeField] private GibManager obstacleGibManager;
    [SerializeField] private GameObject obstaclePrefab;
    private PlayerPowerUp playerPowerUp;
    private new Collider collider;

    void Start() {
        playerPowerUp = GetComponent<PlayerPowerUp>();
        collider = GetComponent<Collider>();
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
                        if (playerPowerUp.AddPowerUp(powerUpObject.Type)) { 
                            powerUpSpawner.DespawnPowerUp(row);
                        }
                    }
                }
            }
            else if (otherObject.CompareTag(TagHolder.GIBS_TAG)) {
                Physics.IgnoreCollision(collider, other, true);
            }
        }
    }

    private void collideWithObstacle(Collider other) {
        //HARDENED POWER-UP OVERRIDE OBSTACLE HITTING
        if (playerPowerUp.GetActivePowerUp() == PowerUpType.Hardened) {
            other.enabled = false;

            GameObject obstacle = other.gameObject;

            //smashing obstacle
            GameObject[] activeGibs = obstacleGibManager.Activate(other.transform.position, other.transform.localScale, true, true);
            

            obstacle.GetComponent<MeshRenderer>().enabled = false;

            playerPowerUp.RemovePowerUp();
            return;
        }

        gameEndManager.endGame();
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
    }
    
}
