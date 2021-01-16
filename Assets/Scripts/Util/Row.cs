using UnityEngine;
using System.Collections;

public class Row : MonoBehaviour {

    //boolean array for row structure
    //TRUE for GAP
    //FALSE for OBSTACLE
    public bool[] structures;

    public PowerUp powerUp;

    public IEnumerator MakeCubesFall(Transform cube, float landedHeight, int initialSpawn) {
        float t = 0f;
        Vector3 position = cube.position;
        float initialY = position.y;

        while (t <= 1) {
            t += (1.5f / (initialSpawn + 1)) * Time.deltaTime;
            position = cube.position;
            position.y = Mathf.Lerp(initialY, landedHeight, t);
            cube.position = position;

            yield return null;
        }
    }

    public bool HasPowerUp () {
        return powerUp != null;
    }

}