using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGibs : MonoBehaviour
{
    private uint DEFAULT_DIVIDE = 4;
    private Quaternion quaternion = new Quaternion();

    public GameObject[] smashCube (GameObject original, GameObject prefab, uint divide) {
        return smashCube(original, prefab, quaternion, divide);
    }

    public GameObject[] smashCube(GameObject original, GameObject prefab, Quaternion rotation, uint divide) {
        if (!IsPowerOfTwo(divide)) divide = DEFAULT_DIVIDE;

        Vector3 originalPos = original.transform.localPosition;
        float originalScale = original.transform.localScale.z;
        original.GetComponent<BoxCollider>().enabled = false;
        original.GetComponent<CharacterController>().enabled = false;
        original.GetComponent<MeshRenderer>().enabled = false;

        float partScale = originalScale / (float) divide;
        uint parts = divide * 16;

        GameObject[] cubeParts = new GameObject[parts];

        int count = 0;
        for (int x = 0; x < divide; x++) {
            for (int y = 0; y < divide; y++) {
                for (int z = 0; z < divide; z++) {
                    float xPos = originalPos.x + ((originalScale / 2f) - (partScale / 2f)) - (x * partScale);
                    float yPos = originalPos.y + ((originalScale / 2f) - (partScale / 2f)) - (y * partScale);
                    float zPos = originalPos.z + ((originalScale / 2f) - (partScale / 2f)) - (z * partScale);

                    cubeParts[count] = spawnCubeGib(prefab, new Vector3(xPos, yPos, zPos), rotation, partScale);
                    count++;
                }
            }
        }
        return cubeParts;
    }

    public GameObject spawnCubeGib(GameObject prefab, Vector3 position, Quaternion rotation, float partScale) {
        GameObject part = Instantiate(prefab, position, rotation);
        Vector3 partScaleVector = part.transform.localScale;
        partScaleVector.x = partScaleVector.y = partScaleVector.z = partScale;
        part.transform.localScale = partScaleVector;
        return part;
    }

    bool IsPowerOfTwo(uint x) {
        return (x != 0) && ((x & (x - 1)) == 0);
    }
}
