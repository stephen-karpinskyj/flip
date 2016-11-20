using System;
using System.Collections.Generic;

[Serializable]
public class TilePath
{
    private List<Tile> tiles = new List<Tile>();

    public bool IsClear()
    {
        return this.tiles.Count <= 0;
    }

    public void Clear()
    {
        this.tiles.Clear();
    }

    public bool Contains(Tile tile)
    {
        return tile != null && this.tiles.Contains(tile);
    }

    public void PushTile(Tile tile, BoardDirection inDirection)
    {
        if (!this.IsValidTile(tile))
        {
            return;
        }

        var lastTile = this.PeekLastTile();

        // First tile
        if (lastTile == null)
        {
            this.AddTile(tile);
            return;
        }

        var nextSection = Board.Instance.Pathfind(lastTile, tile, inDirection);

        foreach (var nextTile in nextSection)
        {
            if (this.IsValidTile(nextTile))
            {
                this.AddTile(nextTile);
            }
        }
    }

    public Tile PopFirstTile()
    {
        if (this.tiles.Count <= 0)
        {
            return null;
        }

        var first = this.PeekFirstTile();
        this.tiles.RemoveAt(0);

        return first;
    }

    public Tile PeekFirstTile()
    {
        return this.tiles.Count <= 0 ? null : this.tiles[0];
    }

    public Tile PeekLastTile()
    {
        return this.tiles.Count <= 0 ? null : this.tiles[this.tiles.Count - 1];
    }

    public Tile PeekNextTile(Tile tile)
    {
        var index = this.tiles.IndexOf(tile) + 1;
        return index <= 0 || index >= this.tiles.Count ? null : this.tiles[index];
    }

    public Tile PeekSecondLastTile()
    {
        return this.tiles.Count <= 1 ? null : this.tiles[this.tiles.Count - 2];
    }

    private void AddTile(Tile tile)
    {
        this.tiles.Add(tile);
    }

    private bool IsValidTile(Tile tile)
    {
        return tile != null && tile != this.PeekLastTile();
    }
}
