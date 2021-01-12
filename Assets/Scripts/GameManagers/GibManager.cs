using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GibManager : MonoBehaviour
{
    [SerializeField] private GameValues gameValues;
    [SerializeField] private GameObject explosionSphere;
    [SerializeField] private Transform poolObject;
    [SerializeField] private GameObject prefab;
    private Stack<GameObject> pool = new Stack<GameObject>();
    private CubeGibsUtil cubeGibsUtil;
    private Vector3 defaultPosition = new Vector3();
    private Quaternion defaultRotation = new Quaternion();

    // Start is called before the first frame update
    void Start() {
        cubeGibsUtil = transform.parent.GetComponent<CubeGibsUtil>();

        //filling pool with objects
        AddPartsToPool(gameValues.Divide * 16);
    }

    void AddPartsToPool (uint num) {
        for (int i = 0; i < num; i++) {
            pool.Push(Instantiate(prefab, defaultPosition, defaultRotation, poolObject));
        }
    }

    public GameObject[] Activate(Vector3 originalPosition, Vector3 originalScale, bool continueGame, bool explode) {
        GameObject holder = new GameObject("Gib Holder");
        holder.transform.parent = transform;

        uint missingParts = (uint) ((gameValues.Divide * 16) - pool.Count);
        if (missingParts > 0) {
            AddPartsToPool(missingParts);
        }

        GameObject[] activeGibs = cubeGibsUtil.SmashCube(pool, originalPosition, originalScale, holder.transform, gameValues.Divide);
        if (continueGame) { 
            StartCoroutine(GibsFade(holder, activeGibs));
            StartCoroutine(MoveHolder(holder));
        }

        if (explode) {
            StartCoroutine(Explode(holder, originalScale.y));
        }

        return activeGibs;
    }

    IEnumerator Explode(GameObject holder, float originalHeight) {
        Vector3 position = holder.transform.position;
        position.y += originalHeight / 2f;

        GameObject explosion = Instantiate(explosionSphere, position, defaultRotation);

        explosion.GetComponent<Collider>().enabled = true;
        yield return new WaitForSeconds(0.5f);
        Destroy(explosion);
    }

    IEnumerator MoveHolder (GameObject holder) {
        while (holder != null && holder.activeInHierarchy && gameValues.GameActive) {
            Vector3 position = transform.position;
            position.x -= gameValues.ForwardSpeed * Time.deltaTime;
            transform.position = position;
            yield return null;
        }
    }

    IEnumerator GibsFade(GameObject holder, GameObject[] activeGibs) {
        //initial delay
        yield return new WaitForSeconds(1);

        //begin fading
        foreach (GameObject gib in activeGibs) {
            gib.GetComponent<Animator>().Play(TagHolder.ANIM_FADE_OUT);
        }

        //wait for animation to finish
        yield return new WaitForSeconds(1);

        //return gibs to pool, remove holder
        foreach (GameObject gib in activeGibs) {
            pool.Push(gib);
            gib.transform.parent = poolObject;
        }
        Destroy(holder);
    }

}
