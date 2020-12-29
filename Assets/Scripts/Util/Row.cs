using UnityEngine;

public class Row {
    //array of obstacles in row
    private GameObject[] obstacles;

    //gaps for the player to go through
    //numbered pairs (z coord, width)
    private float[][] gaps;

    public Row (GameObject[] obstacles, float[][] gaps) {
        this.obstacles = obstacles;
        this.gaps = gaps;
    }

    public GameObject[] getObstacles() {
        return obstacles;
    }

    public float[][] getGaps() {
        return gaps;
    }

}