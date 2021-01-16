using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Class to manage cube smashing effect.
/// </summary>
public class CubeGibsUtil : MonoBehaviour
{
    private uint DEFAULT_DIVIDE = 4;
    private Quaternion quaternion = new Quaternion();

    public GameObject[] SmashCube(Stack<GameObject> parts, Vector3 originalPosition, Vector3 originalScale, Transform parent, uint divide) {
        //ensure divide value is valid
        if (!IsPowerOfTwo(divide)) divide = DEFAULT_DIVIDE;

        //ensure enough parts to build
        uint partsAmt = divide * divide * divide;
        if (parts.Count < partsAmt) throw new System.Exception("Not enough Parts!");

        //resolve scale
        float partScaleX = originalScale.x / (float) divide;
        float partScaleY = originalScale.y / (float) divide;
        float partScaleZ = originalScale.z / (float) divide;
        
        //activate objects
        GameObject[] activeParts = new GameObject[partsAmt];

        int count = 0;
        for (int x = 0; x < divide; x++) {
            for (int y = 0; y < divide; y++) {
                for (int z = 0; z < divide; z++) {
                    float xPos = originalPosition.x + ((originalScale.x / 2f) - (partScaleX / 2f)) - (x * partScaleX);
                    float yPos = originalPosition.y + ((originalScale.y / 2f) - (partScaleY / 2f)) - (y * partScaleY);
                    float zPos = originalPosition.z + ((originalScale.z / 2f) - (partScaleZ / 2f)) - (z * partScaleZ);

                    activeParts[count] = DeployGib(parts.Pop(), new Vector3(xPos, yPos, zPos), parent, partScaleX, partScaleY, partScaleZ, false);
                    count++;
                }
            }
        }
        return activeParts;
    }

    public GameObject DeployGib(GameObject part, Vector3 position, Transform parent, float partScaleX, float partScaleY, float partScaleZ, bool instantiate) {
        if (instantiate) part = Instantiate(part, position, quaternion);
        part.transform.SetParent(parent, false);

        //setting scale
        Vector3 partScaleVector = part.transform.localScale;
        partScaleVector.x = partScaleX;
        partScaleVector.y = partScaleY;
        partScaleVector.z = partScaleZ;
        part.transform.localScale = partScaleVector;

        //setting position
        part.transform.position = position;

        return part;
    }

    bool IsPowerOfTwo(uint x) {
        return (x != 0) && ((x & (x - 1)) == 0);
    }
}
