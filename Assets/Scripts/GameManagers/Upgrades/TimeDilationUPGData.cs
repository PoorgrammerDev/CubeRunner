using UnityEngine;

[CreateAssetMenu(fileName = "New TimeDilationUPGData", menuName = "Upgrade Stats/Time Dilation Upgrade Data")]
public class TimeDilationUPGData : ScriptableObject {
    public TimeDilationUPGEntry[] leftPath;
    public TimeDilationUPGEntry[] rightPath;
}

[System.Serializable]
public struct TimeDilationUPGEntry {
    public float duration;
    public float slowRate;
}