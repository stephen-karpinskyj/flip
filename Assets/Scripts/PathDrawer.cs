using System;
using UnityEngine;

[Serializable]
public class PathDrawer
{
    private Tile lastPressedTile;
    private Tile currentPressedTile;

    public readonly TilePath Path = new TilePath();
    public Tile AllowedStartingTile { get; set; }
    public BoardDirection StartingDirection { get; set; }

    public void CheckInput()
    {
        var isDown = Input.anyKey;

        if (isDown)
        {
            this.currentPressedTile = Board.Instance.GetTile(Input.mousePosition);
            var pressingLastTile = this.currentPressedTile == this.lastPressedTile;

            if (!pressingLastTile)
            {
                var pressingStartTile = this.currentPressedTile == this.AllowedStartingTile;

                if (pressingStartTile)
                {
                    this.Path.Clear();
                }

                var lastTile = this.AllowedStartingTile;
                var inDirection = this.StartingDirection;
                var fromDirection = inDirection;

                if (this.Path.IsClear())
                {
                    this.Path.PushTile(this.AllowedStartingTile, null, inDirection);
                }
                else
                {
                    // Change from direction to be from end of drawn path
                    lastTile = this.Path.PeekLastTile();
                    fromDirection.Set(this.Path.PeekSecondLastTile(), lastTile);
                }

                inDirection.Set(lastTile, this.currentPressedTile, fromDirection);
                this.Path.PushTile(this.currentPressedTile, this.AllowedStartingTile, inDirection);
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

            //t.Border.Highlight(t == this.AllowedStartingTile);

            t.Backing.Highlight(shouldHighlight);
        });
    }
}
