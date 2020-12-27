using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{

    private Transform transform;

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private GameObject cubePrefab;

    [SerializeField]
    private int amount = 10;

    [SerializeField]
    private int spawnDistanceMinimum = 5;

    [SerializeField]
    private float cubeSizeLower = 0.5f;

    [SerializeField]
    private float cubeSizeUpper = 3f;

    private float x_upper = 90;
    private float z_bounds = 4;

    [SerializeField]
    private float moveSpeed;

    private HashSet<GameObject> obstacles;

    

    // Start is called before the first frame update
    void Start()
    {
        obstacles = new HashSet<GameObject>();
        transform = GetComponent<Transform>();
        
        //TODO edit spawning script to continually spawn as cubes decrease,
        //and spawn using a different algorithm that accounts for rows not being entirely blocked (possible to beat) and not too close to each other and definitely not clipping into each other

        for (int i = 0; i < amount; i++) {
            float cubeSize = Random.Range(cubeSizeLower, cubeSizeUpper);
            Vector3 position = getSpawnPosition(cubeSize);

            GameObject newCube = Instantiate(cubePrefab, position, new Quaternion(), transform);

            Vector3 scale = newCube.transform.localScale;
            scale.x = cubeSize;
            scale.y = cubeSize;
            scale.z = cubeSize;
            newCube.transform.localScale = scale;

            obstacles.Add(newCube);
        }

    }

    void Update() {
        MoveCubes();
    }

    void MoveCubes() {
        if (obstacles != null) {
            foreach (GameObject obstacle in obstacles) {
                //check if passed player
                Transform transform = obstacle.transform;
                if (transform.position.x < 0) {
                    obstacles.Remove(obstacle);
                    Destroy(obstacle);
                    continue;
                }

                //move if not passed
                Rigidbody rigidbody = obstacle.GetComponent<Rigidbody>();
                Vector3 velocity = rigidbody.velocity;
                velocity.x -= moveSpeed * Time.deltaTime * 10;
                rigidbody.velocity = velocity;
            }
        }
    }
    
    Vector3 getSpawnPosition(float size) {
        float x_lower = player.transform.position.x + spawnDistanceMinimum;

        float x = Random.Range(x_lower, x_upper);
        float y = size / 2;
        float z = Random.Range(-z_bounds, z_bounds);
        
        return new Vector3(x, y, z);
    }

/*
    boolean checkPositionViable(Vector3 vector) {
        
    }
    */
}
