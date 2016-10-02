using UnityEngine;
using DG.Tweening;

public class Tile : BaseMonoBehaviour
{
    [SerializeField]
    private TileHighlightable backing;

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

    public void SetPosition(Vector2 position)
    {
        this.transform.localPosition = position;
    }

    public void Punch()
    {
        this.transform.DOPunchScale(this.punchTweenerData);
    }
}
