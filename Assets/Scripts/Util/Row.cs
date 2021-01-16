using UnityEngine;
using System.Collections;

public class Row : MonoBehaviour {

    //boolean array for row structure
    //TRUE for GAP
    //FALSE for OBSTACLE
    public bool[] structures;

    public PowerUp powerUp;

    public IEnumerator MakeCubesFall(Transform cube, Material transparent, Material opaque, float landedHeight, int initialSpawn) {
        //transparent material
        Renderer renderer = cube.GetComponent<Renderer>();
        renderer.material = transparent;

        float t = 0f;
        Vector3 position = cube.position;
        float initialY = position.y;

        Color color = renderer.material.color;
        color.a = 0;
        while (t <= 1) {
            t += (1.5f / (initialSpawn + 1)) * Time.deltaTime;
            position = cube.position;
            position.y = Mathf.Lerp(initialY, landedHeight, t);
            cube.position = position;

            color.a = Mathf.Lerp(0, 1, t);
            renderer.material.color = color;

            yield return null;
        }

        renderer.material = opaque;
    }

    public bool HasPowerUp () {
        return powerUp != null;
    }

}