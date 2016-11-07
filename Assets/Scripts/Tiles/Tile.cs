using UnityEngine;
using DG.Tweening;

public class Tile : BaseMonoBehaviour
{
    public enum HighwayMode
    {
        IsNot = 0,
        MightBe,
        Is,
    }

    [SerializeField]
    private TileHighlightable backing;

    [SerializeField]
    private TileHighlightable backing2;

    [SerializeField]
    private TileHighlightable border;

    [SerializeField]
    private PunchTweenerData punchTweenerData;

    public TileHighlightable Backing
    {
        get { return this.backing; }
    }

    public TileHighlightable Border
    {
        get { return this.border; }
    }

    public Vector2I Coordinates { get; set; }

    public HighwayMode CurrentHighwayMode { get; private set; }

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

    public void SetHighwayMode(HighwayMode mode)
    {
        this.CurrentHighwayMode = mode;

        if (mode == HighwayMode.IsNot)
        {
            this.backing.Highlight(false);
            this.backing2.Highlight(false);
        }
        else if (mode == HighwayMode.MightBe)
        {
            this.backing2.Highlight(true);
        }
        else if (mode == HighwayMode.Is)
        {
            this.backing.Highlight(true);
        }
    }
}
