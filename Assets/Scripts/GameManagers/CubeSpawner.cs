﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles calculating and deploying new obstacles
/// </summary>
public class CubeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private PowerUpSpawner powerUpSpawner;
    [SerializeField] private GameValues gameValues;
    [SerializeField] private Transform groundPlane;
    [SerializeField] private Material transparentMaterial;
    [SerializeField] private Material opaqueMaterial;
    [SerializeField] private BitsSpawner bitsSpawner;
    [SerializeField] private Decimate decimate;
    [SerializeField] private int rowCount = 10;

    private float firstRowDistance;
    private int lanes;
    public int Lanes => lanes;

    private Transform cubePoolObject;
    private Stack<GameObject> cubePool;
    private LinkedList<Row> rows;
    public LinkedList<Row> Rows => rows;

    private Vector3 spawnPosition = new Vector3(0, 0.5f, 0);
    private Quaternion quaternion = new Quaternion();

    // Start is called before the first frame update
    void Start() {
        cubePoolObject = transform.GetChild(0);
        lanes = (int) ((groundPlane.localScale.z * 10f) / gameValues.WidthScale);
        rows = new LinkedList<Row>();
        firstRowDistance = 15 + (1.5f * gameValues.ForwardSpeed);

        InstantiateObstacles();
    }

    //instantiate objects at the start
    void InstantiateObstacles() {
        int initPoolSize = (lanes / 2) * rowCount;
        cubePool = new Stack<GameObject>(initPoolSize);
        for (int i = 0; i < initPoolSize; i++) {
            cubePool.Push(Instantiate(cubePrefab, spawnPosition, quaternion, cubePoolObject));
        }
    }

    //spawns in the rows and begins the game
    public void InitialSpawn() {
        CreateNewRow(firstRowDistance, 0);
        for (int i = 0; i < rowCount; i++) {
            CreateNewRow(-1, (i + 1));
        }
    }

    //gets a cube from pool, or makes a new one if empty
    GameObject getCube() {
        if (cubePool.Count > 0) {
            return cubePool.Pop();
        }
        return Instantiate(cubePrefab, spawnPosition, quaternion, cubePoolObject);
    }

    //This method creates a completely new row object and adds it to the queue.
    //This is not to be confused with InitiateRow which takes an already made but inactive row and puts it in the game field.
    Row CreateNewRow(float xCoordSpawn, int initialSpawnNum) {
        Row row = new GameObject("Row", typeof(Row)).GetComponent<Row>();
        row.transform.SetParent(transform);

        InitiateRow(row, xCoordSpawn, initialSpawnNum, -1, false /*TODO: set this back to true*/, false);
        return row;
    }

    public void InitiateRow(Row row) {
        InitiateRow(row, -1, -1, -1, false, false);
    }

    //This method takes an already made but inactive row and "activates" it, putting it into the game field.
    //This is not to be confused with CreateNewRow which adds an extra row to the total.
    public void InitiateRow(Row row, float xCoordSpawn, int initialSpawn, int gaps, bool ignorePUP, bool ignoreBits) {
        if (gaps == -1) GetPlacementArray(row);
        else GetPlacementArray(row, gaps);

        if (xCoordSpawn == -1) xCoordSpawn = GetNextXCoord(row);

        //move row to desired X coordinate
        Vector3 rowPosition = row.transform.position;
        rowPosition.x = xCoordSpawn;
        row.transform.position = rowPosition;

        for (int i = 0; i < lanes; i++) {
            if (!row.structures[i]) {
                //get cube, set row as parent
                GameObject cube = getCube();
                cube.transform.SetParent(row.transform, false);
                cube.SetActive(true);

                float height = Random.Range(1.5f, 3f);
                
                //change cube position
                Vector3 cubePos = cube.transform.localPosition;
                cubePos.x = 0;
                cubePos.y = (initialSpawn != -1) ? (15 + (10 * initialSpawn)) + Random.Range(-5, 5) : (height / 2f);
                cubePos.z = (((lanes) / 2f) - i - 0.5f) * gameValues.WidthScale;

                cube.transform.localPosition = cubePos;

                //change cube scale
                Vector3 cubeScale = cube.transform.localScale;
                cubeScale.y = height;
                cubeScale.z = gameValues.WidthScale;
                cube.transform.localScale = cubeScale;

                if (initialSpawn != -1) {
                    StartCoroutine(row.MakeCubesFall(cube.transform, transparentMaterial, opaqueMaterial, height / 2f, initialSpawn));
                }
            }
        }

        //Power ups
        if (!ignorePUP && powerUpSpawner.IsReady() && Random.value < gameValues.PowerUpSpawnChance) {
            int slot = -1;
            do {
                slot = Random.Range(0, lanes);
            } while (!row.structures[slot]);

            powerUpSpawner.SpawnPowerUp(row, slot, lanes, PowerUpType.Decimate); //TODO: remove
        }

        //Bits
        //TODO: Change AMOUNT to non-literal values (use vars instead), and perhaps have it affected by game progression
        else if (!ignoreBits && initialSpawn != 0 && Random.value < gameValues.BitsSpawnChance) {
            row.bits = bitsSpawner.SpawnBits(Random.Range(2, 5), row, rows.Last.Value, (BitsPattern) Random.Range(0, (int) BitsPattern.COUNT), initialSpawn != -1);
        }

        rows.AddLast(row);
    }

    void GetPlacementArray(Row row) {
        GetPlacementArray(row, GetGapAmount(1));
    }

    //randomly generates the gaps
    void GetPlacementArray(Row row, int gaps) {
        //generate boolean array
        bool[] placement = new bool[lanes];

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

        row.structures = placement;
        row.gapCount = gaps;
    }

    void Update() {
        if (!gameValues.GameActive) return;

        MoveRows();
    }

    //moves the rows
    void MoveRows() {
        bool removeFlag = false;
        foreach (Row row in rows) {
            //check if passed player, recycle
            Transform transform = row.transform;
            if (transform.position.x < -1f) {
                removeFlag = true;
                continue;
            }
            
            Vector3 position = transform.position;
            position.x -= gameValues.ForwardSpeed * Time.deltaTime;
            transform.position = position;
        }

        if (removeFlag) {
            Row first = rows.First.Value;
            rows.RemoveFirst();
            RecycleRow(first, true);

            if (!gameValues.PassedFirstObstacle) {
                gameValues.PassedFirstObstacle = true;
            }

            if (!powerUpSpawner.IsDeployed()) {
                powerUpSpawner.TickCooldown();
            }
        }
    }

    //Method run on Row after it passes the Player. Recycles it and puts it onto the end,
    public void RecycleRow(Row row, bool autoInit) {
        Transform rowTransform = row.transform;

        //If row has Power up, detaches it and deactivates it
        if (row.HasPowerUp()) {
            powerUpSpawner.DespawnPowerUp(row);
        }

        //If row has bits, stashes them
        if (row.HasBits()) {
            bitsSpawner.StashBits(row);
        }

        //return cubes to respective pools
        while (rowTransform.childCount > 0) {
            Transform child = rowTransform.GetChild(rowTransform.childCount - 1);
            if (child.CompareTag(TagHolder.OBSTACLE_TAG)) {
                child.SetParent(cubePoolObject);
                cubePool.Push(child.gameObject);
            }

            //stash decimate objects
            else if (child.CompareTag(TagHolder.DECIMATE_OBJ_TAG)) {
                decimate.StashObject(child.gameObject);
            }

            else {
                Debug.LogError("Invalid Object " + child.name + " in Row");
                Destroy(child.gameObject);
            }
        }

        row.structures = null;
        if (autoInit) InitiateRow(row);
    }

    float GetNextXCoord (Row row) {
        float minTriangleZSide = float.MinValue;
        
        Row previous = rows.Last.Value;
        //finding the shortest possible Z gap dist
        for (int i = 0; i < previous.structures.Length; i++) {
            for (int j = 0; j < row.structures.Length; j++) {
                float previousStructureZ = (((lanes) / 2f) - i - 0.5f) * gameValues.WidthScale;
                float currentStructureZ = (((lanes) / 2f) - j - 0.5f) * gameValues.WidthScale;

                float triangleZSide = Mathf.Abs(previousStructureZ - currentStructureZ);
                if (triangleZSide > minTriangleZSide) {
                    minTriangleZSide = triangleZSide;
                }
            }
        }

        if (minTriangleZSide == float.MaxValue) throw new System.Exception("Error in gap length calculation.");
        
        float minDist = Mathf.Max((gameValues.ForwardSpeed * minTriangleZSide) / gameValues.StrafingSpeed, 2f);
        float distance = minDist * Random.Range(gameValues.RowDistMultLowerBound, gameValues.RowDistMultUpperBound);

        return (previous.transform.position.x + distance);
    }

    int GetGapAmount(int currentNumber) {
        if (currentNumber < (lanes - 1) && Random.value < gameValues.GapIncreaseChance) {
            return GetGapAmount(++currentNumber);
        }
        else {
            return currentNumber;
        }
    }

}
