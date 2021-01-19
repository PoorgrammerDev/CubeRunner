using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundObjects : MonoBehaviour
{
    [SerializeField] private GameValues gameValues;
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Transform poolParentObj;
    [SerializeField] private int poolStartingAmt;
    [SerializeField] private int startingAmt;
    [SerializeField] private float maxRot;

    private Transform[] poolObjs;
    private Stack<BGObject>[] pools;
    private Quaternion defaultRot = new Quaternion();
    private WaitForSeconds wait = new WaitForSeconds(0.25f);
    private WaitForSeconds wait2 = new WaitForSeconds(0.01f);
    
    private int types;

    // Start is called before the first frame update
    void Start() {
        types = prefabs.Length;
        pools = new Stack<BGObject>[types];
        poolObjs = new Transform[types];

        for (int i = 0; i < types; i++) {
            //create pool and instantiate objs
            poolObjs[i] = new GameObject("BG_OBJ_POOL_" + i).transform;
            pools[i] = new Stack<BGObject>();
            poolObjs[i].transform.SetParent(poolParentObj);
            AddPartsToPool(i, poolStartingAmt);
        }

        
    }

    public void Initialize() {
        for (int i = 0; i < startingAmt - 1; i++) {
            StartCoroutine(Deploy(true));
        }
        StartCoroutine(DeployMechanism());
    }

    void AddPartsToPool (int type, int num) {
        if (type >= 0 && type < prefabs.Length) {
            for (int i = 0; i < num; i++) {
                pools[type].Push(new BGObject(type, Instantiate(prefabs[type], Vector3.zero, defaultRot, poolObjs[type].transform)));
            }
        }   
    }

    BGObject GetObject(int type) {
        if (pools[type].Count > 0) {
            return pools[type].Pop();
        }
        AddPartsToPool(type, 1);
        return GetObject(type);
    }

    public IEnumerator DeployMechanism() {
        do {
            StartCoroutine(Deploy(false));
            yield return wait;
        } while (gameValues.GameActive);
    }

    IEnumerator Deploy(bool init) {
        BGObject bgObject = GetObject(Random.Range(0, types));
            GameObject innerObj = bgObject.GameObject;

            //size
            float size = Random.Range(0.25f, 3f);

            //set into position
            float x = Random.Range(init ? 10 : 125, 150);
            float z = Random.Range(10, 150);

            float y = Random.Range(z > 25 ? 0 : 5, 25);

            //possibility to flip to right side
            if (Random.Range(0, 100) < 50) z = -z;

            innerObj.transform.position = new Vector3(x, y, z);

            //set rotation values
            if (Random.Range(0, 100) < 50) {
                bgObject.Rotate = true;
                bgObject.Rotation = new Vector3(Random.Range(-maxRot, maxRot), Random.Range(-maxRot, maxRot), Random.Range(-maxRot, maxRot));
            }

            //de-parents from pool, which automatically activates it
            innerObj.transform.SetParent(transform);

            //inflates up to size
            float t = 0f;
            float currentSize;
            while (t <= 1) {
                t += 0.25f * Time.deltaTime;
                currentSize = Mathf.Lerp(0, size, t);
                innerObj.transform.localScale = new Vector3(currentSize, currentSize, currentSize);
                yield return null;
            }

            StartCoroutine(ObjectTravel(bgObject, size, z));
    }

    IEnumerator ObjectTravel(BGObject bgObject, float size, float z) {
        GameObject innerObj = bgObject.GameObject;
        Transform bgTransform = innerObj.transform;
        z = Mathf.Abs(z);

        //calculating move speed
        float speed = Random.Range(0.01f, 0.15f / size) / (z / 150f);

        Vector3 move = new Vector3(-speed, 0, 0);

        do {
            //move forward
            bgTransform.Translate(move, Space.World);

            //rotate
            if (bgObject.Rotate) {
                bgTransform.Rotate(bgObject.Rotation, Space.Self);
            }
            yield return wait2;
        } while (gameValues.GameActive && bgTransform.position.x > 0);

        //return to pool
        innerObj.transform.SetParent(poolObjs[bgObject.Type]);
        pools[bgObject.Type].Push(bgObject);
    }
}
