using UnityEngine;

public abstract class BaseMonoBehaviour : MonoBehaviour
{
    public new Transform transform { get; private set; }

    protected virtual void Awake()
    {
        this.transform = this.GetComponent<Transform>();
    }
}
