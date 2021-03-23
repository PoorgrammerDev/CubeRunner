using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BitsPattern {
    Direct,
    StraightMiddle,
    StraightSplit,
    COUNT
}

public class BitsSpawner : MonoBehaviour
{
    [SerializeField] private GameValues gameValues;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform poolObject;
    [SerializeField] private Material regularMat;
    [SerializeField] private Material transparentMat;
    private Stack<GameObject> bitsPool = new Stack<GameObject>();
    const float BIT_SPAWN_HEIGHT = 0.5f;

    private Quaternion rotation = new Quaternion();

    // Start is called before the first frame update
    void Start() {
        rotation.eulerAngles = new Vector3(45, 45, 45);
        AddPartsToPool(35);
    }

    void AddPartsToPool (int num) {
        for (int i = 0; i < num; i++) {
            bitsPool.Push(Instantiate(prefab, Vector3.zero, rotation, poolObject));
        }
    }

    GameObject GetBit() {
        if (bitsPool.Count > 0) {
            return bitsPool.Pop();
        }

        AddPartsToPool(1);
        return GetBit();
    }

    public GameObject[] SpawnBits(int amount, Row row, Row lastRow, BitsPattern pattern, bool fadeIn) {
        int length = row.structures.Length;
        Vector3 currentRowTarget = new Vector3(row.transform.position.x, BIT_SPAWN_HEIGHT, (((length) / 2f) - GetRandomGap(row, length) - 0.5f) * gameValues.WidthScale); //deducing the Z is some arbitrary formula from CubeSpawner.cs
        Vector3 lastRowTarget = new Vector3(lastRow.transform.position.x, BIT_SPAWN_HEIGHT, (((length) / 2f) - GetRandomGap(lastRow, length) - 0.5f) * gameValues.WidthScale); //deducing the Z is some arbitrary formula from CubeSpawner.cs

        GameObject[] bits = new GameObject[amount];
        switch (pattern) {
            case BitsPattern.Direct:
                //draw line directly from two pts
                DirectLine(currentRowTarget, lastRowTarget, row.transform, amount, bits, 0);
                break;
            case BitsPattern.StraightMiddle:
                //get the mid-way Z between 2 pts, draw one straight line there
                float middleZ = (currentRowTarget.z + lastRowTarget.z) / 2f;
                currentRowTarget.z = lastRowTarget.z = middleZ;

                DirectLine(currentRowTarget, lastRowTarget, row.transform, amount, bits, 0);
                break;
            case BitsPattern.StraightSplit:
                //get mid-way X between 2 pts, draw one line from last to there, and one line from current to there
                float middleX = (currentRowTarget.x + lastRowTarget.x) / 2f;

                Vector3 middleTargetCurrent = currentRowTarget;
                Vector3 middleTargetLast = lastRowTarget;
                middleTargetCurrent.x = middleTargetLast.x = middleX;

                DirectLine(currentRowTarget, middleTargetCurrent, row.transform, amount / 2, bits, 0);
                DirectLine(lastRowTarget, middleTargetLast, row.transform, amount - (amount / 2), bits, amount / 2);
                break;
        }

        //fade in
        if (fadeIn) {
            foreach (GameObject bit in bits) {
                StartCoroutine(FadeInBit(bit));
            }
        }

        return bits;
    }

    void DirectLine(Vector3 currentRowTarget, Vector3 lastRowTarget, Transform rowTransform, int amount, GameObject[] bits, int startingPt) {
        Vector3 directLine = (currentRowTarget - lastRowTarget) / (amount + 1);
        for (int i = 0; i < amount; i++) {
            bits[startingPt + i] = GetBit();
            bits[startingPt + i].transform.position = currentRowTarget - (directLine * (i + 1));
            bits[startingPt + i].transform.parent = rowTransform;
        }
    }

    public void StashBits(Row row) {
        foreach (GameObject bit in row.bits) {
            bit.transform.parent = poolObject;
            bit.transform.position = Vector3.zero;
            bit.SetActive(true);
            bitsPool.Push(bit);
        }
        row.bits = null;
    }

    int GetRandomGap(Row row, int length) {
        int value;
        do {
            value = Random.Range(0, length);
        } while (!row.structures[value]);
        return value;
    }

    IEnumerator FadeInBit(GameObject bit) {
        float t = 0f;
        Renderer renderer = bit.GetComponent<Renderer>();
        renderer.material = transparentMat;

        Color color = renderer.material.GetColor("_Color");

        while (t <= 1) {
            t += 3f * Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, t);
            renderer.material.SetColor("_Color", color);

            yield return null;
        }

        renderer.material = regularMat;
    }
}
