using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    [SerializeField]
    private GameObject activePlayer;

    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private GameValues gameValues;

    [SerializeField]
    private uint divide = 2;

    private uint DEFAULT_DIVIDE = 8;

    void Start() {
        if (!IsPowerOfTwo(divide)) {
            divide = DEFAULT_DIVIDE;
        }
    }

    public void endGame() {
        smashCube();
        StartCoroutine(DisableGame());
    }

    IEnumerator DisableGame() {
        yield return new WaitForSeconds(0.1f);
        gameValues.setGameActive(false);
    }

    public void smashCube() {
        Vector3 activePlayerPos = activePlayer.transform.localPosition;
        float activePlayerScale = activePlayer.transform.localScale.z;
        activePlayer.GetComponent<BoxCollider>().enabled = false;
        activePlayer.GetComponent<CharacterController>().enabled = false;
        activePlayer.GetComponent<MeshRenderer>().enabled = false;

        float partScale = activePlayerScale / (float) divide;
        uint parts = divide * 4;

        Quaternion quaternion = new Quaternion();
        for (int x = 0; x < divide; x++) {
            for (int y = 0; y < divide; y++) {
                for (int z = 0; z < divide; z++) {
                    float xPos = activePlayerPos.x + ((activePlayerScale / 2f) - (partScale / 2f)) - (x * partScale);
                    float yPos = activePlayerPos.y + ((activePlayerScale / 2f) - (partScale / 2f)) - (y * partScale);
                    float zPos = activePlayerPos.z + ((activePlayerScale / 2f) - (partScale / 2f)) - (z * partScale);

                    GameObject part = Instantiate(playerPrefab, new Vector3(xPos, yPos, zPos), quaternion);
                    Vector3 partScaleVector = part.transform.localScale;
                    partScaleVector.x = partScaleVector.y = partScaleVector.z = partScale;
                    part.transform.localScale = partScaleVector;
                }
            }
        }
    }

    bool IsPowerOfTwo(uint x) {
        return (x != 0) && ((x & (x - 1)) == 0);
    }
}
