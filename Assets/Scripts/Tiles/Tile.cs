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
    private MeshRenderer backingRenderer;

    [SerializeField]
    private TileHighlighter backing;

    [SerializeField]
    private Vector2I colourRange;

    [SerializeField]
    private PunchTweenerData punchTweenerData;

    public Vector2I Coordinates { get; set; }

    private float defaultColorScale;
    private Vector3 defaultScale;

    public bool IsHighlighted
    {
        get { return this.backing.IsHighlighted; }
    }

    private void Awake()
    {
        var c = (byte)Random.Range(this.colourRange.x, this.colourRange.y);
        this.defaultColorScale = c / 255f;
        var color = new Color32(c, c, c, byte.MaxValue);
        this.backingRenderer.material.color = color;

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

    public void Highlight(bool highlight)
    {
        this.backing.Highlight(this.defaultColorScale, highlight);
    }
}
