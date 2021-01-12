using UnityEngine;

/// <summary>
/// Class to manage cube smashing effect.
/// </summary>
public class CubeGibs : MonoBehaviour
{
    private uint DEFAULT_DIVIDE = 4;
    private Quaternion quaternion = new Quaternion();

    public GameObject[] smashCube (GameObject original, GameObject prefab, uint divide) {
        return smashCube(original, prefab, quaternion, null, divide);
    }

    public GameObject[] smashCube(GameObject original, GameObject prefab, Quaternion rotation, Transform parent, uint divide) {
        if (!IsPowerOfTwo(divide)) divide = DEFAULT_DIVIDE;

        Vector3 originalPos = original.transform.position;
        float originalScaleX = original.transform.localScale.x;
        float originalScaleY = original.transform.localScale.y;
        float originalScaleZ = original.transform.localScale.z;

        float partScaleX = originalScaleX / (float) divide;
        float partScaleY = originalScaleY / (float) divide;
        float partScaleZ = originalScaleZ / (float) divide;
        uint parts = divide * 16;

        GameObject[] cubeParts = new GameObject[parts];

        int count = 0;
        for (int x = 0; x < divide; x++) {
            for (int y = 0; y < divide; y++) {
                for (int z = 0; z < divide; z++) {
                    float xPos = originalPos.x + ((originalScaleX / 2f) - (partScaleX / 2f)) - (x * partScaleX);
                    float yPos = originalPos.y + ((originalScaleY / 2f) - (partScaleY / 2f)) - (y * partScaleY);
                    float zPos = originalPos.z + ((originalScaleZ / 2f) - (partScaleZ / 2f)) - (z * partScaleZ);

                    cubeParts[count] = spawnCubeGib(prefab, new Vector3(xPos, yPos, zPos), rotation, parent, partScaleX, partScaleY, partScaleZ);
                    count++;
                }
            }
        }
        return cubeParts;
    }

    public GameObject spawnCubeGib(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, float partScale) {
        return spawnCubeGib(prefab, position, rotation, parent, partScale, partScale, partScale);
    }

    public GameObject spawnCubeGib(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, float partScaleX, float partScaleY, float partScaleZ) {
        GameObject part = parent != null ? Instantiate(prefab, position, rotation, parent) : Instantiate(prefab, position, rotation);
        Vector3 partScaleVector = part.transform.localScale;
        partScaleVector.x = partScaleX;
        partScaleVector.y = partScaleY;
        partScaleVector.z = partScaleZ;
        part.transform.localScale = partScaleVector;
        return part;
    }

    bool IsPowerOfTwo(uint x) {
        return (x != 0) && ((x & (x - 1)) == 0);
    }
}
