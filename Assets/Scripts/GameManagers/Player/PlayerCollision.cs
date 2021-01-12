using UnityEngine;
using System.Collections;

/// <summary>
/// Handles player collision with objects, like obstacles or power-ups.
/// </summary>
public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private EndGame gameEndManager;
    [SerializeField] private PowerUpSpawner powerUpSpawner;
    [SerializeField] private ObstacleGibHolder obstacleGibHolder;
    [SerializeField] private CubeGibs cubeGibs;
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
                        powerUpSpawner.DespawnPowerUp(row);
                        playerPowerUp.AddPowerUp(powerUpObject.Type);
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
            obstacleGibHolder.enabled = true;
            GameObject[] gibs = cubeGibs.smashCube(obstacle, obstaclePrefab, new Quaternion(), obstacleGibHolder.transform, gameEndManager.Divide);
            obstacle.GetComponent<MeshRenderer>().enabled = false;

            //explosion force
            foreach (GameObject gib in gibs) {
                gib.GetComponent<Rigidbody>().AddExplosionForce(2, transform.position, 5, 1, ForceMode.Impulse);
            }
            StartCoroutine(CheckForPassedGibs(gibs));

            playerPowerUp.RemovePowerUp();
            return;
        }

        gameEndManager.endGame();
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
    }

    IEnumerator CheckForPassedGibs(GameObject[] gibs) {
        float maxX = float.MinValue;
        WaitForSeconds wait = new WaitForSeconds(1);
        do {
            foreach (GameObject gib in gibs) {
                if (gib.transform.position.x > maxX) {
                    maxX = gib.transform.position.x;
                }
            }
            yield return wait;
        } while (maxX > 0);
        
        for (int i = gibs.Length - 1; i > 0; i--) {
            Destroy(gibs[i]);
        }

        obstacleGibHolder.enabled = false;
        Vector3 pos = obstacleGibHolder.transform.position;
        pos.x = 0;
        obstacleGibHolder.transform.position = pos;
    }
    
}
