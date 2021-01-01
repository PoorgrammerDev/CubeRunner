using UnityEngine;
using UnityEngine.UI;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField]
    private EndGame gameEndManager;

    private void OnTriggerEnter(Collider other) {
        GameObject otherObject = other.gameObject;
        if (otherObject != gameObject) {
            if (otherObject.CompareTag(TagHolder.OBSTACLE_TAG)) {
                if (otherObject.transform.position.x > 0) {
                    collideWithObstacle();
                }
            }
            else if (otherObject.CompareTag(TagHolder.POWERUP_TAG)) {

            }
        }
    }

    private void collideWithObstacle() {
        //TODO add power up exceptions here

        gameEndManager.endGame();
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
    }
    
}
