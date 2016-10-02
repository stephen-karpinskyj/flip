using System;
using System.Collections.Generic;

[Serializable]
public class TilePath
{
    private List<Tile> tiles = new List<Tile>();
    private Tile lastPushedTile;

    public bool IsClear()
    {
        return this.tiles.Count <= 0;
    }

    public void Clear()
    {
        this.tiles.Clear();
        this.lastPushedTile = null;
    }

    public bool Contains(Tile tile)
    {
        return tile != null && this.tiles.Contains(tile);
    }

    public bool PushTile(Tile tile)
    {
        if (!this.IsValidTile(tile))
        {
            return false;
        }

        // First tile
        if (this.lastPushedTile == null)
        {
            this.AddTile(tile);
            return true;
        }

        var pushedAnyTiles = false;

        var secondLastTile = this.tiles.Count > 1 ? this.tiles[this.tiles.Count - 2] : null;
        var nextSection = Board.Instance.Pathfind(this.lastPushedTile, tile, secondLastTile);

        foreach (var nextTile in nextSection)
        {
            if (this.IsValidTile(nextTile))
            {
                this.AddTile(nextTile);
                pushedAnyTiles = true;
            }
        }

        return pushedAnyTiles;
    }

    public Tile PopFirstTile()
    {
        if (this.tiles.Count <= 0)
        {
            return null;
        }

        var first = this.tiles[0];
        this.tiles.RemoveAt(0);

        if (this.tiles.Count <= 0)
        {
            this.lastPushedTile = null;
        }

        return first;
    }

    public Tile PeekFirstTile()
    {
        return this.tiles.Count <= 0 ? null : this.tiles[0];
    }

    private void AddTile(Tile tile)
    {
        if (this.Contains(tile))
        {
            this.TrimToTile(tile);
        }

        this.tiles.Add(tile);
        this.lastPushedTile = tile;
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
            this.lastPushedTile = this.tiles.Count > 0 ? this.tiles[this.tiles.Count - 1] : null;
        }
    }

    private bool IsValidTile(Tile tile)
    {
        return tile != null && tile != this.lastPushedTile;
    }
}
