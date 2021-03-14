using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Decimate : AbstractPowerUp
{
    [Header("References")]
    [SerializeField] private PlayerPowerUp powerUpManager;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private CubeSpawner cubeSpawner;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform poolObject;
    private Stack<GameObject> pool = new Stack<GameObject>();
    private Quaternion quaternion = new Quaternion();

    [Header("Options")]
    [SerializeField] private int poolSize;
    

    void Start() {
        AddPartsToPool(poolSize);
    }
    
    void AddPartsToPool (int num) {
        for (int i = 0; i < num; i++) {
            pool.Push(Instantiate(prefab, Vector3.zero, quaternion, poolObject));
        }
    }

    GameObject GetDecimateObject() {
        if (pool.Count > 0) {
            return pool.Pop();
        }
        AddPartsToPool(1);
        return GetDecimateObject();
    }

    public void RunDecimate() {
        foreach (Row row in cubeSpawner.Rows) { //TODO: add count
            int count = cubeSpawner.Lanes - row.gapCount;
            Transform obstacle = row.transform.GetChild(Random.Range(0, count));
            if (obstacle.CompareTag(TagHolder.OBSTACLE_TAG)) {
                StartCoroutine(DecimateObstacle(row, obstacle));
            }
            else {
                print(obstacle.name); //TODO: REMOVE THIS PRIOR TO RELEASE
            }
        }
        powerUpManager.RemovePowerUp();
    }

    IEnumerator DecimateObstacle(Row row, Transform obstacle) {
        //get decimate object
        GameObject decimateObj = GetDecimateObject();

        //match pos and scl
        decimateObj.transform.position = obstacle.position;
        decimateObj.transform.localScale = obstacle.localScale;

        //swap out
        obstacle.gameObject.SetActive(false);
        decimateObj.transform.SetParent(row.transform, true);

        //wait until deactivated (handled by animation event)
        while (decimateObj.activeInHierarchy) {
            yield return null;
        }

        StashObject(decimateObj);
    }

    public void StashObject (GameObject decimateObject) {
        if (decimateObject.transform.parent.Equals(poolObject)) return;

        decimateObject.transform.SetParent(poolObject);
        decimateObject.SetActive(true);
    }
}
