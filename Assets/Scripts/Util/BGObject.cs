using UnityEngine;

public class BGObject
{
    private int type;
    public int Type { get => type; set => type = value; }
    
    private GameObject gameObject;
    public GameObject GameObject { get => gameObject; set => gameObject = value; }

    private bool rotate;
    public bool Rotate { get => rotate; set => rotate = value; }

    private Vector3 rotation;
    public Vector3 Rotation { get => rotation; set => rotation = value; }

    public BGObject(int type, GameObject gameObject)
    {
        this.type = type;
        this.gameObject = gameObject;
    }

}
