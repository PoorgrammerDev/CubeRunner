using UnityEngine;

/// <summary>
/// Row class represents each row of obstacles during the Game.
/// </summary>
public class Row : MonoBehaviour {
    //array of obstacles in row
    public GameObject[] obstacleObjects;

    //boolean array for gaps / obstacles
    //TRUE represents GAP
    //FALSE represents OBSTACLE
    public bool[] obstaclePlacement;
}