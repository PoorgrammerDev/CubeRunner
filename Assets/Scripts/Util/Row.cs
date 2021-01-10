using UnityEngine;
using System.Collections;

public class Row : MonoBehaviour {

    //boolean array for row structure
    //TRUE for GAP
    //FALSE for OBSTACLE
    public bool[] structures;

    public PowerUp powerUp;

    public IEnumerator MakeCubesFall(GameObject cube, float landedHeight) {
        Rigidbody rigidbody = cube.GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;

        while (cube != null && cube.transform.position.y > landedHeight) {
            yield return null;
        }

        rigidbody.isKinematic = true;
        yield break;
    }

    public bool HasPowerUp () {
        return powerUp != null;
    }

}