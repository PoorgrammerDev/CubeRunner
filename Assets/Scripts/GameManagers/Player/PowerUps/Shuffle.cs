using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuffle : AbstractPowerUp
{
    [SerializeField] private PlayerPowerUp powerUpManager;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private CubeSpawner cubeSpawner;

    //TODO: make it so power ups cant spawn in regenerated rows and also some funky stuff happens when you regenerate the starting rows
    public void RunShuffle() {
        LinkedList<Row> rows = cubeSpawner.Rows;
        Queue<Row> emptyRows = new Queue<Row>();
        Queue<int> gaps = new Queue<int>();

        //remove the immediate row
        Row first = rows.First.Value;
        cubeSpawner.RecycleRow(first, false);
        rows.RemoveFirst();

        //get x of the one after it
        float startingX = rows.First.Value.transform.position.x;

        foreach (Row row in rows) {
            //Record gaps to queue
            gaps.Enqueue(row.gapCount);

            //Remove row
            cubeSpawner.RecycleRow(row, false);

            //Add row to queue
            emptyRows.Enqueue(row);
        }

        //Clear row list
        while (rows.First != null) {
            rows.RemoveFirst();
        }

        //Create first row
        cubeSpawner.InitiateRow(emptyRows.Dequeue(), startingX, -1, Mathf.Min(gaps.Dequeue() + 1, cubeSpawner.Lanes - 1), true);

        //Create all subsquent rows
        while (emptyRows.Count > 0 && gaps.Count > 0) {
            cubeSpawner.InitiateRow(emptyRows.Dequeue(), -1, -1, Mathf.Min(gaps.Dequeue() + 1, cubeSpawner.Lanes - 1), false);
        }
        
        cubeSpawner.InitiateRow(first, -1, -1, -1, false);
        powerUpManager.RemovePowerUp();
    }
}
