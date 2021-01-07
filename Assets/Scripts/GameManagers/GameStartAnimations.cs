using UnityEngine;

/// <summary>
/// Handles Starting Game animaions (Game scene-side)
/// </summary>
public class GameStartAnimations : MonoBehaviour
{
    [SerializeField] private GameValues gameValues;
    [SerializeField] private CubeSpawner cubeSpawner;
    [SerializeField] private Treadmill treadmill;
    [SerializeField] private GameObject playerCube;
    [SerializeField] private GameObject sun;

    // Start is called before the first frame update
    void Start() {
        
    }
}
