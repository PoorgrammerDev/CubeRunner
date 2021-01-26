using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundObjects : MonoBehaviour
{
    [SerializeField] private GameValues gameValues;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolAmount;
    [SerializeField] private float maxRot;

    private Stack<BGObject> pool;
    private Quaternion defaultRot = new Quaternion();
    private WaitForSeconds wait = new WaitForSeconds(0.25f);
    private WaitForSeconds wait2 = new WaitForSeconds(0.01f);
    private int adjustedPoolAmount;

    // Start is called before the first frame update
    void Start() {
        pool = new Stack<BGObject>();

        adjustedPoolAmount = poolAmount * PlayerPrefs.GetInt(TagHolder.PREF_GRAPHICS, 2);
        if (adjustedPoolAmount > 0) {
            AddPartsToPool(adjustedPoolAmount);
        }
    }

    public void Initialize() {
        if (adjustedPoolAmount > 0) {
            for (int i = 0; i < (adjustedPoolAmount / 2) - 1; i++) {
                StartCoroutine(Deploy(true));
            }
            StartCoroutine(DeployMechanism());
        }
    }

    void AddPartsToPool (int num) {
        for (int i = 0; i < num; i++) {
            GameObject gameObject = Instantiate(prefab, Vector3.zero, defaultRot, transform);
            gameObject.SetActive(false);

            pool.Push(new BGObject(gameObject));
        } 
    }

    BGObject GetObject() {
        if (pool.Count > 0) {
            return pool.Pop();
        }
        return null;
    }

    public IEnumerator DeployMechanism() {
        do {
            StartCoroutine(Deploy(false));
            yield return wait;
        } while (gameValues.GameActive);
    }

    IEnumerator Deploy(bool init) {
        BGObject bgObject = GetObject();
        if (bgObject != null) {
            GameObject innerObj = bgObject.GameObject;
            innerObj.SetActive(true);

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

        if (gameValues.GameActive) {
            //return to pool
            pool.Push(bgObject);
            innerObj.SetActive(false);
        }
    }
}
