using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles calculating and deploying new obstacles
/// </summary>
public class CubeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private GameValues gameValues;
    [SerializeField] private Transform groundPlane;
    [SerializeField] private int rowCount = 10;

    private float firstRowDistance;
    private int lanes;
    private Row[] rows;

    private Transform cubePoolObject;
    private Stack<GameObject> cubePool;

    private Vector3 spawnPosition = new Vector3(0, 0.5f, 0);
    private Quaternion quaternion = new Quaternion();

    // Start is called before the first frame update
    void Start() {
        cubePoolObject = transform.GetChild(0);
        lanes = (int) (groundPlane.localScale.z * 10);
        rows = new Row[rowCount];
        firstRowDistance = 10 + (1.5f * gameValues.ForwardSpeed);

        CreateObjects();
        SpawnRow(rows[0], 10);
    }

    //instantiate objects at the start
    void CreateObjects() {
        for (int i = 0; i < rowCount; i++) {
            rows[i] = new GameObject("Row", typeof(Row)).GetComponent<Row>();
            rows[i].transform.SetParent(transform);
        }

        int initPoolSize = (lanes / 2) * rowCount;
        cubePool = new Stack<GameObject>(initPoolSize);
        for (int i = 0; i < initPoolSize; i++) {
            cubePool.Push(Instantiate(cubePrefab, spawnPosition, quaternion, cubePoolObject));
        }
    }

    //gets a cube from pool, or makes a new one if empty
    GameObject getCube() {
        GameObject cube = cubePool.Pop();
        if (cube != null) {
            return cube;
        }
        return Instantiate(cubePrefab, spawnPosition, quaternion, cubePoolObject);
    }

    void SpawnRow(Row row, float xCoordSpawn) {
        row.obstaclePlacement = getPlacementArray();

        //move row to desired X coordinate
        Vector3 rowPosition = row.transform.position;
        rowPosition.x = xCoordSpawn;
        row.transform.position = rowPosition;

        for (int i = 0; i < lanes; i++) {
            if (!row.obstaclePlacement[i]) {
                //get cube, set row as parent
                GameObject cube = getCube();
                cube.transform.SetParent(row.transform, false);
                
                //change cube position
                Vector3 cubePos = cube.transform.position;
                cubePos.z = ((float) lanes / 2f) - i - 0.5f;
                cube.transform.position = cubePos;
            }
        }
    }

    bool[] getPlacementArray() {
        //generate boolean array
        bool[] placement = new bool[lanes];
        int gaps = Random.Range(1, lanes);

        //randomly fill gaps
        for (int i = 0; i < gaps; i++) {
            int place = Random.Range(0, lanes);
            if (!placement[place]) {
                placement[place] = true;
            }
            else {
                i--;
            }
        }
        return placement;
    }





}
