using UnityEngine;

public abstract class AbstractPowerUp : MonoBehaviour
{
    [SerializeField] private Color color;
    public Color Color => color;

    [SerializeField] private Sprite sprite;
    public Sprite Sprite => sprite;

    [SerializeField] private AudioClip[] sounds;
    public AudioClip[] Sounds => sounds;

    [SerializeField] private float[] soundStartTimes;
    public float[] SoundStartTimes => soundStartTimes;
}
