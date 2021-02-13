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
    private Hardened hardenedPUP;
    private new Collider collider;

    public LayerMask obstacleLayer;

    void Start() {
        playerPowerUp = GetComponent<PlayerPowerUp>();
        collider = GetComponent<Collider>();
        hardenedPUP = GetComponent<Hardened>();
    }

    private void OnTriggerEnter(Collider other) {
        GameObject otherObject = other.gameObject;
        if (otherObject != gameObject) {
            if (otherObject.CompareTag(TagHolder.OBSTACLE_TAG)) {
                if (otherObject.transform.position.x > 0) {
                    CollideWithObstacle();
                }
            }
            else if (otherObject.CompareTag(TagHolder.POWERUP_TAG)) {
                Physics.IgnoreCollision(collider, other, true);
                
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

    private void CollideWithObstacle() {
        //HARDENED POWER-UP OVERRIDE OBSTACLE HITTING
        if (playerPowerUp.GetActivePowerUp() == PowerUpType.Hardened) {
            Collider[] allObstacles = Physics.OverlapBox(transform.position, transform.localScale / 2f, Quaternion.identity, obstacleLayer, QueryTriggerInteraction.Collide);

            foreach (Collider obstacle in allObstacles) {
                //smashing obstacle
                if (gameValues.Divide != 0) {
                    obstacleGibManager.Activate(obstacle.transform.position, obstacle.transform.localScale, true, true);
                }

                obstacle.gameObject.SetActive(false);
            }

            hardenedPUP.PlaySound();
            playerPowerUp.RemovePowerUp();
            return;
        }

        gameEndManager.endGame((PlayerPrefs.GetInt(TagHolder.PREF_SKIP_ANIM) == 0) && gameValues.Divide != 0);
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
    }
    
}
