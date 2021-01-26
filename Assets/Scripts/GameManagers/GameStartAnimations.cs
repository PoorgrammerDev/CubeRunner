using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Handles Starting Game animaions (Game scene-side)
/// </summary>
public class GameStartAnimations : MonoBehaviour
{
    [SerializeField] private GameValues gameValues;
    [SerializeField] private CubeSpawner cubeSpawner;
    [SerializeField] private Treadmill treadmill;
    [SerializeField] private GameObject HUD;
    [SerializeField] private GameObject sun;
    [SerializeField] private BackgroundObjects backgroundObjects;

    // Start is called before the first frame update
    void Start() {
        StartCoroutine(StartingAnim());
    }

    IEnumerator StartingAnim() {
        if (PlayerPrefs.GetInt(TagHolder.PREF_SKIP_ANIM) == 1) {
            treadmill.transform.position = Vector3.zero;
            treadmill.active = true;
            
            sun.SetActive(true);
            StartGame();
        }
        else {
            //Start Treadmill (Also activates Game and spawns in cubes at the end)
            StartCoroutine(ActivateTreadmill());

            //Sun fades in
            yield return new WaitForSeconds(1);
            sun.SetActive(true);
        }
    }

    IEnumerator ActivateTreadmill() {
        float t = 0.0f;
        float initialSpeed = (190f / gameValues.ForwardSpeed);
        treadmill.active = true;

        do {
            treadmill.Speed = Mathf.Lerp(initialSpeed, 1f, t);
            t += 0.5f * Time.deltaTime;
            
            yield return null;
        } while (treadmill.Speed > 1.0f);


        //start the game
        yield return new WaitForSeconds(1);
        StartGame();
    }

    void StartGame() {
        cubeSpawner.InitialSpawn();
        backgroundObjects.Initialize();
        gameValues.GameActive = true;
        HUD.SetActive(true);
    }
    
}
