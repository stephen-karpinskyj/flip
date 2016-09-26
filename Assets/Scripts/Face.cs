using UnityEngine;

// TEMP
public class Face : BaseMonoBehaviour
{
    [SerializeField]
    private Renderer rend;

    protected override void Awake()
    {
        base.Awake();
        
        var c = Random.Range(0.5f, 1f);
        this.rend.material.color = new Color(c, c, c);
    }

    public void Destroy()
    {
        Object.Destroy(this.gameObject);
    }
}
