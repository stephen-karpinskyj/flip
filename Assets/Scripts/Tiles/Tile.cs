using UnityEngine;
using DG.Tweening;

public class Tile : BaseMonoBehaviour
{
    public enum Mode
    {
        IsNot = 0,
        MightBe,
        Is,
    }

    [SerializeField]
    private Highlightable backing;

    [SerializeField]
    private Highlightable backing2;

    [SerializeField]
    private Highlightable border;

    [SerializeField]
    private PunchTweenerData punchTweenerData;

    public Highlightable DarkBacking
    {
        get { return this.backing; }
    }

    public Highlightable LightBacking
    {
        get { return this.backing2; }
    }

    public Highlightable Border
    {
        get { return this.border; }
    }

    public Vector2I Coordinates { get; set; }

    private Vector3 defaultScale;

    private void Awake()
    {
        this.defaultScale = this.transform.localScale;
    }

    public void SetPosition(Vector2 position)
    {
        this.transform.localPosition = position;
    }

    public void Punch()
    {
        this.transform.DOPunchScale(this.punchTweenerData, this.defaultScale);
    }
}
