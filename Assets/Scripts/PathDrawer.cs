using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PathDrawer
{
    private bool wasDown;
    private bool isDrawingPath;

    public Tile AllowedStartingTile { get; set; }

    private List<Tile> path = new List<Tile>();
    private Tile lastTile;

    public IEnumerable<Tile> CurrentPath
    {
        get { return this.path; }
    }

    public void CheckForInput()
    {
        var isDown = Input.anyKey;
        var onDown = isDown && !wasDown;
        var onUp = !isDown && this.wasDown;
        this.wasDown = isDown;

        var tile = Board.Instance.GetTile(Input.mousePosition);

        if (onDown)
        {
            if (tile == this.AllowedStartingTile)
            {
                this.OnStartDrawing(tile);
            }
            else
            {
                this.OnEndDrawing(tile);
            }
        }
        else if (onUp)
        {
            this.OnEndDrawing(tile);
        }
        else if (isDown)
        {
            this.OnContinueDrawing(tile);
        }

        Board.Instance.ForEachTile(t => t.Border.Highlight(this.path.Contains(t)));
    }

    private void OnStartDrawing(Tile tile)
    {
        if (this.isDrawingPath)
        {
            return;
        }

        this.isDrawingPath = true;

        this.path.Clear();
        this.AddTileToPath(tile);
    }

    private void OnContinueDrawing(Tile tile)
    {
        if (!this.isDrawingPath)
        {
            return;
        }

        var hasChangedTile = tile != this.lastTile;

        if (tile == null || (hasChangedTile && this.path.Contains(tile)))
        {
            this.OnEndDrawing(tile);
        }
        else
        {
            var nextPathSection = Board.Instance.Pathfind(lastTile, tile);
            foreach (var nextTile in nextPathSection)
            {
                if (nextTile == lastTile)
                {
                    continue;
                }

                this.AddTileToPath(nextTile);
            }
        }
    }

    private void OnEndDrawing(Tile tile)
    {
        if (!this.isDrawingPath)
        {
            return;
        }

        this.isDrawingPath = false;

        if (this.lastTile != tile && tile != null)
        {
            this.AddTileToPath(tile);
        }
    }

    private void AddTileToPath(Tile tile)
    {
        this.path.Add(tile);
        this.lastTile = tile;
    }

    public Tile PopFirstTile()
    {
        if (this.path.Count <= 0)
        {
            return null;
        }

        var first = this.path[0];
        this.path.RemoveAt(0);
        return first;
    }
}
