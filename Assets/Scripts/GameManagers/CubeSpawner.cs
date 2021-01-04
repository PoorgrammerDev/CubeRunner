using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    private new Transform transform;

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private GameObject cubePrefab;

    [SerializeField]
    private GameValues gameValues;
    private float firstRowDistance;

    [SerializeField]
    private float obstacleMinHeight = 1.5f;

    [SerializeField]
    private float obstacleMaxHeight = 3f;

    [SerializeField]
    private float obstacleThickness = 1f;

    [SerializeField]
    private int maxGaps = 3;

    [SerializeField]
    private Transform groundPlane;

    private float zBorders;

    private List<Row> rows;

    // Start is called before the first frame update
    void Start()
    {
        rows = new List<Row>();
        transform = GetComponent<Transform>();
        zBorders = groundPlane.localScale.z * 5;
        firstRowDistance = 10 + (1.5f * gameValues.ForwardSpeed);
        
        initialSpawn();
    }


    private void initialSpawn() {
        float[][] gaps = getGaps();
        rows.Add(spawnRow(firstRowDistance, gaps, true, 0)); 

        for (int i = 0; i < 5; i++) {
            spawnNextRow(true, i + 1);
        }
    }

    private void spawnNextRow() {
        spawnNextRow(false, -1);
    }

    private void spawnNextRow(bool spawnFromAbove, int aboveMult) {
        float[][] gaps = getGaps();
        Row previous = rows[rows.Count - 1];
        rows.Add(spawnRow(previous.getObstacles()[0].transform.position.x + getDistance(previous.getGaps(), gaps), gaps, spawnFromAbove, aboveMult));
    }

    private Row spawnRow(float xCoordSpawn, float[][] gaps, bool spawnFromAbove, int aboveMult) {
        //for (int i = 0; i < gaps.Length; i++)
        //{
        //    GameObject ye = Instantiate(this.gap, new Vector3(xCoordSpawn, 1, gaps[i][0]), new Quaternion());
        //    ye.name = "Gap " + i;
        //
        //    Vector3 scale = ye.transform.localScale;
        //    scale.z = gaps[i][1];
        //    ye.transform.localScale = scale;
        //}
        
        GameObject[] obstacles = new GameObject[gaps.Length + 1];
        for (int i = 0; i < gaps.Length + 1; i++)
        {
            float positiveBound = (i > 0) ? gaps[i-1][0] - (gaps[i-1][1] / 2f) : zBorders;
            float negativeBound = (i < gaps.Length) ? gaps[i][0] + (gaps[i][1] / 2f) : -zBorders;

            float obstacleSize = Mathf.Abs(positiveBound - negativeBound);
            GameObject obstacle = spawnObstacle(new Vector3(xCoordSpawn, obstacleSize / 2f, (positiveBound + negativeBound) / 2f), obstacleSize, spawnFromAbove, aboveMult);
            obstacles[i] = obstacle;
        }

        return new Row(obstacles, gaps);
    }

    private GameObject spawnObstacle (Vector3 position, float size, bool spawnFromAbove, int aboveMult) {
        float height = Mathf.Clamp(size, obstacleMinHeight, obstacleMaxHeight);
        position.y = spawnFromAbove ? 15 + (10 * aboveMult) : height / 2f;

        GameObject cube = Instantiate(cubePrefab, position, new Quaternion(), transform);
        if (spawnFromAbove) {
            StartCoroutine(makeCubesFall(cube, height / 2f));
        }

        Vector3 scale = cube.transform.localScale;
        scale.x = obstacleThickness;
        scale.y = height;
        scale.z = size;

        cube.transform.localScale = scale;
        return cube;
    }

    IEnumerator makeCubesFall(GameObject cube, float landedHeight) {
        Rigidbody rigidbody = cube.GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;

        while (cube != null && cube.transform.position.y > landedHeight) {
            yield return null;
        }

        rigidbody.isKinematic = true;
        yield break;
    }


    private float getDistance (float[][] previousGaps, float[][] gaps) {
        float minTriangleZSide = float.MinValue;
        
        //finding the shortest possible Z gap dist
        for (int i = 0; i < previousGaps.Length; i++) {
            for (int j = 0; j < gaps.Length; j++) {
                float triangleZSide = Mathf.Abs(previousGaps[i][0] - gaps[j][0]);
                if (triangleZSide > minTriangleZSide) {
                    minTriangleZSide = triangleZSide;
                }
            }
        }

        if (minTriangleZSide == float.MaxValue) throw new System.Exception("Error in gap length calculation.");
        
        float minDist = Mathf.Max((gameValues.ForwardSpeed * minTriangleZSide) / gameValues.StrafingSpeed, 2f);
        return minDist * Random.Range(gameValues.RowDistMultLowerBound, gameValues.RowDistMultUpperBound);
    }

    private float[][] getGaps() {
        int amount = Random.Range(1, maxGaps + 1);
        
        float remainingWidth = zBorders * gameValues.WidthMultiplier;
        float[][] gaps = new float[amount][];
        for (int i = 0; i < gaps.Length; i++) {
            float[] currentGap = new float[2];

            //scale or width
            currentGap[1] = Random.Range(gameValues.WidthMultiplier, (remainingWidth * 0.75f) - (gaps.Length - i - 1));
            remainingWidth -= currentGap[1];


            List<List<float>> possibleLandings = new List<List<float>>();
            for (int j = 0; j < i + 1; j++) {
                float positiveBound = ((j > 0) ? gaps[j-1][0] - (gaps[j-1][1] / 2f) : zBorders); 
                float negativeBound = ((j < i) ? gaps[j][0] + (gaps[j][1] / 2f) : -zBorders);

                if (Mathf.Abs(positiveBound - negativeBound) >= currentGap[1]) {
                    List<float> possibleLanding = new List<float>();
                    possibleLanding.Add(positiveBound);
                    possibleLanding.Add(negativeBound);

                    possibleLandings.Add(possibleLanding);
                }
            }

            if (possibleLandings.Count > 0) {
                int rand = Random.Range(0, possibleLandings.Count);
                List<float> landing = possibleLandings[rand];
                float adjustedPosBound = landing[0] - (currentGap[1] / 2f);
                float adjustedNegBound = landing[1] + (currentGap[1] / 2f);

                currentGap[0] = Random.Range(adjustedNegBound, adjustedPosBound);
                gaps[i] = currentGap;
            }
        }
        return sortGaps(gaps);
    }

    private float[][] sortGaps (float[][] gaps) {
        float[] temp;
        for (int i = 0; i < gaps.Length; i++) {
            for (int j = i + 1; j < gaps.Length; j++) {
                if (gaps[i][0] < gaps[j][0]) {
                    temp = gaps[j];
                    gaps[j] = gaps[i];
                    gaps[i] = temp;
                }
            }
        }
        return gaps;
    }

    void Update() {
        if (!gameValues.GameActive) return;
        
        MoveCubes();
    }

    void MoveCubes() {
        if (rows != null) {
            for (int i = 0; i < rows.Count; i++) {
                bool delete = false;
                foreach (GameObject obstacle in rows[i].getObstacles()) {
                    //check if passed player, mark for deletion
                    Transform transform = obstacle.transform;
                    if (transform.position.x < -1f) {
                        delete = true;
                        break;
                    }

                    Vector3 position = transform.position;
                    position.x -= gameValues.ForwardSpeed * Time.deltaTime;
                    transform.position = position;
                }

                //delete row and spawn a new one
                if (delete) {
                    if (!gameValues.PassedFirstObstacle) {
                        gameValues.PassedFirstObstacle = true;
                    }

                    foreach (GameObject obstacle in rows[i].getObstacles()) {
                        Destroy(obstacle);
                    }
                    rows.RemoveAt(i);
                    spawnNextRow();
                }
            }
        }
    }
    
    public int getMaxGaps() {
        return maxGaps;
    }

    public void setMaxGaps(int num) {
        if (num >= 1) {
            maxGaps = num;
        }
        else {
            throw new System.Exception("Max Gaps must be at least one.");
        }
    }

}
