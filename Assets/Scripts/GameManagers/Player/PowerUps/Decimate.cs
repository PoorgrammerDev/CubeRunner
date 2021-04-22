using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decimate : AbstractPowerUp
{
    [Header("References")]
    [SerializeField] private PlayerPowerUp powerUpManager;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private CubeSpawner cubeSpawner;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform poolObject;
    [SerializeField] private DecimateUPGData upgradeData;
    [SerializeField] private SaveManager saveManager;
    

    [Header("Options")]
    [SerializeField] private int poolSize;
    [SerializeField] private int rowCount;
    
    private Stack<GameObject> pool = new Stack<GameObject>();
    private Quaternion quaternion = new Quaternion();

    void Start() {
        SetupUpgradeData();
        AddPartsToPool(poolSize);
    }

    void SetupUpgradeData() {
        int upgLevel = saveManager.GetUpgradeLevel(PowerUpType.Decimate, 0);
    
        DecimateUPGEntry upgradeEntry = upgradeData.leftPath[upgLevel];
        this.rowCount = upgradeEntry.rows;
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
        //play audio
        audioSource.clip = Sounds[0];
        audioSource.time = SoundStartTimes[0];
        audioSource.Play();

        int count = 0;
        foreach (Row row in cubeSpawner.Rows) {
            //skip the first row (the one that the player is about to pass)
            if (count == 0) {
                count++;
                continue;
            }

            //check count
            if (count > rowCount) break;
            
            int obstacleCount = cubeSpawner.Lanes - row.gapCount;
            Transform obstacle = row.transform.GetChild(Random.Range(0, obstacleCount));
            if (obstacle.CompareTag(TagHolder.OBSTACLE_TAG)) {
                StartCoroutine(DecimateObstacle(row, obstacle));
            }
            count++;
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
