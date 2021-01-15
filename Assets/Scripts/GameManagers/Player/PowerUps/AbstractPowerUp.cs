using UnityEngine;

public abstract class AbstractPowerUp : MonoBehaviour
{
    [SerializeField] private Color color;
    public Color Color => color;

    [SerializeField] private Sprite sprite;
    public Sprite Sprite => sprite;
}
