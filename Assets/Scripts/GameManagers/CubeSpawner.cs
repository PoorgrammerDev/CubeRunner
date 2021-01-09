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
    [SerializeField] private float widthScale = 1;

    private float firstRowDistance;
    private int lanes;

    private Transform cubePoolObject;
    private Stack<GameObject> cubePool;
    private Stack<Row> rows;

    private Vector3 spawnPosition = new Vector3(0, 0.5f, 0);
    private Quaternion quaternion = new Quaternion();

    // Start is called before the first frame update
    void Start() {
        cubePoolObject = transform.GetChild(0);
        lanes = (int) ((groundPlane.localScale.z * 10f) / widthScale);
        rows = new Stack<Row>();
        firstRowDistance = 10 + (1.5f * gameValues.ForwardSpeed);

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
        CreateNewRow(firstRowDistance);
        for (int i = 0; i < rowCount; i++) {
            CreateNewRow(-1);
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
    Row CreateNewRow(float xCoordSpawn) {
        Row row = new GameObject("Row", typeof(Row)).GetComponent<Row>();
        row.transform.SetParent(transform);

        InitiateRow(row, xCoordSpawn);
        rows.Push(row);

        return row;
    }

    //This method takes an already made but inactive row and "activates" it, putting it into the game field.
    //This is not to be confused with CreateNewRow which adds an extra row to the total.
    void InitiateRow(Row row, float xCoordSpawn) {
        row.structures = getPlacementArray();
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

                float height = Random.Range(0.75f, 2.5f);
                
                //change cube position
                Vector3 cubePos = cube.transform.position;
                cubePos.y = height / 2f;
                cubePos.z = (((lanes) / 2f) - i - 0.5f) * widthScale;
                cube.transform.position = cubePos;

                //change cube scale
                Vector3 cubeScale = cube.transform.localScale;
                cubeScale.y = height;
                cubeScale.z = widthScale;
                cube.transform.localScale = cubeScale;
            }
        }
    }

    //randomly generates the gaps
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

    void Update() {
        if (!gameValues.GameActive) return;

        MoveRows();
    }

    //moves the rows
    void MoveRows() {
        foreach (Row row in rows) {
            //check if passed player, recycle
            Transform transform = row.transform;
            if (transform.position.x < -1f) {
                RecycleRow(row);
                continue;
            }
            
            Vector3 position = transform.position;
            position.x -= gameValues.ForwardSpeed * Time.deltaTime;
            transform.position = position;
        }
    }

    //Method run on Row after it passes the Player. Recycles it and puts it onto the end,
    void RecycleRow(Row row) {
        foreach (Transform child in row.transform) {
            child.SetParent(cubePoolObject);
            cubePool.Push(child.gameObject);
        }
        InitiateRow(row, GetNextXCoord(row));
        print(rows.ToString());
    }

    float GetNextXCoord (Row row) {
        float minTriangleZSide = float.MinValue;
        
        Row previous = rows.Peek();
        //finding the shortest possible Z gap dist
        for (int i = 0; i < previous.structures.Length; i++) {
            for (int j = 0; j < row.structures.Length; j++) {
                float previousStructureZ = (((lanes) / 2f) - i - 0.5f) * widthScale;
                float currentStructureZ = (((lanes) / 2f) - j - 0.5f) * widthScale;

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

}
