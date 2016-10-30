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

    public void PushTile(Tile tile, Tile tileToClearOn, BoardDirection inDirection)
    {
        if (!this.IsValidTile(tile))
        {
            return;
        }

        var lastTile = this.PeekLastTile();

        // First tile
        if (lastTile == null)
        {
            this.AddTile(tile, tileToClearOn);
            return;
        }

        var nextSection = Board.Instance.Pathfind(lastTile, tile, inDirection);

        foreach (var nextTile in nextSection)
        {
            if (this.IsValidTile(nextTile))
            {
                this.AddTile(nextTile, tileToClearOn);
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

    public Tile PeekSecondLastTile()
    {
        return this.tiles.Count <= 1 ? null : this.tiles[this.tiles.Count - 2];
    }

    public void ForEachTile(Action<Tile> action)
    {
        foreach (var tile in this.tiles)
        {
            action(tile);
        }
    }

    private void AddTile(Tile tile, Tile tileToClearOn)
    {
        if (this.Contains(tile))
        {
            this.TrimToTile(tile);
        }

        if (tile == tileToClearOn)
        {
            this.Clear();
        }

        this.tiles.Add(tile);
    }

    private void TrimToTile(Tile tile)
    {
        if (tile == null)
        {
            return;
        }

        var startTrimIndex = this.tiles.IndexOf(tile);
        var countToTrim = this.tiles.Count - startTrimIndex;

        if (startTrimIndex >= 0 && countToTrim > 0)
        {
            this.tiles.RemoveRange(startTrimIndex, countToTrim);
        }
    }

    private bool IsValidTile(Tile tile)
    {
        return tile != null && tile != this.PeekLastTile();
    }
}
