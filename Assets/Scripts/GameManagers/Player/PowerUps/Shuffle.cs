using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Shuffle : AbstractPowerUp
{
    [SerializeField] private PlayerPowerUp powerUpManager;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private CubeSpawner cubeSpawner;

    [SerializeField] private Volume volume;
    ColorAdjustments colorAdj;
    LensDistortion lensDist;


    void Start() {
        volume.sharedProfile.TryGet<ColorAdjustments>(out colorAdj);
        volume.sharedProfile.TryGet<LensDistortion>(out lensDist);
    }


    public IEnumerator RunShuffle() {
        powerUpManager.RemovePowerUp();
        float t = 1f;

        Time.timeScale = 0.25f;
        lensDist.active = true;
        colorAdj.active = true;
        while (t >= 0f) {
            t -= 3f * Time.deltaTime;
            lensDist.intensity.SetValue(new NoInterpClampedFloatParameter(Mathf.Lerp(-0.5f, 0, t), -1, 1, true));
            colorAdj.saturation.SetValue(new ClampedFloatParameter(Mathf.Lerp(-100, 0, t), -100, 100, true));
            yield return null;
        }

        colorAdj.contrast.SetValue(new ClampedFloatParameter(50, -100, 100, true));
        ShuffleRows();
        yield return null;

        while (t <= 1) {
            t += 60f * Time.deltaTime;
            lensDist.intensity.SetValue(new NoInterpClampedFloatParameter(Mathf.Lerp(-0.5f, 0, t), -1, 1, true));
            colorAdj.saturation.SetValue(new ClampedFloatParameter(Mathf.Lerp(-100, 0, t), -100, 100, true));
            yield return null;
        }
        colorAdj.contrast.SetValue(new ClampedFloatParameter(0, -100, 100, true));
        lensDist.active = false;
        colorAdj.active = false;
        Time.timeScale = 1;
    }

    public void ShuffleRows() {
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
        cubeSpawner.InitiateRow(emptyRows.Dequeue(), startingX, -1, Mathf.Min(gaps.Dequeue() + 1, cubeSpawner.Lanes - 1), true, true);

        //Create all subsquent rows
        while (emptyRows.Count > 0 && gaps.Count > 0) {
            cubeSpawner.InitiateRow(emptyRows.Dequeue(), -1, -1, Mathf.Min(gaps.Dequeue() + 1, cubeSpawner.Lanes - 1), true, false);
        }
        
        cubeSpawner.InitiateRow(first);
    }
}
