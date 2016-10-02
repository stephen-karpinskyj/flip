using System;
using UnityEngine;

[Serializable]
public class PathDrawer
{
    private Tile lastPressedTile;
    private Tile currentPressedTile;

    public readonly TilePath Path = new TilePath();
    public Tile AllowedStartingTile { get; set; }

    public void CheckInput()
    {
        var isDown = Input.anyKey;

        if (isDown)
        {
            this.currentPressedTile = Board.Instance.GetTile(Input.mousePosition);
            var pressingLastTile = this.currentPressedTile != this.lastPressedTile;

            if (!pressingLastTile)
            {
                var pressingStartTile = this.currentPressedTile == this.AllowedStartingTile;

                if (pressingStartTile)
                {
                    this.Path.Clear();
                }

                if (this.Path.IsClear())
                {
                    this.Path.PushTile(this.AllowedStartingTile);
                }

                this.Path.PushTile(this.currentPressedTile);
            }
        }

        this.lastPressedTile = isDown ? this.currentPressedTile : null;

        Board.Instance.ForEachTile(t =>
        {
            var shouldHighlight = this.Path.Contains(t);

            if (shouldHighlight != t.Backing.IsHighlighted)
            {
                t.Punch();
            }

            t.Backing.Highlight(shouldHighlight);
        });
    }
}
