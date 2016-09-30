using UnityEngine;

public class Tile : BaseMonoBehaviour
{
    [SerializeField]
    private TileHighlightable backing;

    [SerializeField]
    private TileHighlightable border;

    public TileHighlightable Backing
    {
        get { return this.backing; }
    }

    public TileHighlightable Border
    {
        get { return this.border; }
    }

    public void SetPosition(Vector2 position)
    {
        this.transform.localPosition = position;
    }
}
