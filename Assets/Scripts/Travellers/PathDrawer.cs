using System;
using UnityEngine;

[Serializable]
public class PathDrawer
{
    public readonly TilePath Path = new TilePath();
    public Tile AllowedStartingTile { get; set; }
    public BoardDirection StartingDirection { get; set; }

    public void CheckInput()
    {
        var isDown = Input.anyKeyDown;

        if (isDown)
        {
            var pressedTile = Board.Instance.GetTile(Input.mousePosition);
            BoardDirection dir;
            var allPickupsInPath = this.IsAllForegroundPickupsInPath();
            var canReachTile = this.CanReachTile(pressedTile, allPickupsInPath, out dir);

            if (canReachTile)
            {
                if (!MusicManager.Instance.Player.HasStarted)
                {
                    MusicManager.Instance.Player.Play();
                }

                if (this.Path.IsClear())
                {
                    this.Path.PushTile(this.AllowedStartingTile, this.StartingDirection);
                }

                this.Path.PushTile(pressedTile, dir);
            }
        }

        this.UpdatePickups();
        this.UpdateTiles();
    }

    private void UpdatePickups()
    {
        var allPickupsInPath = this.IsAllForegroundPickupsInPath();

        // HACK
        if (allPickupsInPath)
        {
            var track = MusicManager.Instance.Player.CurrentFocusedTrack;
            MusicManager.Instance.Player.SpawnNextLevelPatterns(track, false);
        }

        Board.Instance.ForEachPickup(pickup =>
        {
            pickup.Hide(!pickup.IsForeground && !allPickupsInPath);

            BoardDirection dir;
            var canReachPickup = this.CanReachTile(pickup.CurrentTile, allPickupsInPath, out dir);
            var isInPath = this.Path.Contains(pickup.CurrentTile);
            pickup.SetPickupable(canReachPickup || isInPath);
        });
    }

    private void UpdateTiles()
    {
        Board.Instance.ForEachTile(tile =>
        {
            var shouldHighlight = this.Path.Contains(tile);

            if (shouldHighlight != tile.IsHighlighted)
            {
                tile.Punch();
            }

            tile.Highlight(shouldHighlight);
        });
    }

    private bool CanReachTile(Tile tile, bool includeBackground, out BoardDirection inDirection)
    {
        inDirection = this.StartingDirection;

        var tileIsInPath = this.Path.Contains(tile);

        if (tileIsInPath)
        {
            return false;
        }

        var tileHasPickup = Board.Instance.HasPickup(tile, includeBackground);

        if (!tileHasPickup)
        {
            return false;
        }

        var lastTile = this.Path.PeekLastTile() ?? this.AllowedStartingTile;
        var fromDirection = inDirection;

        if (!this.Path.IsClear())
        {
            // Change from direction to be from end of drawn path
            fromDirection.Set(this.Path.PeekSecondLastTile(), lastTile);
        }

        inDirection.Set(lastTile, tile, fromDirection);

        var pathSection = Board.Instance.Pathfind(lastTile, tile, inDirection);
        var canPathfindToTile = true;

        foreach (var t in pathSection)
        {
            if (t != lastTile && this.Path.Contains(t))
            {
                canPathfindToTile = false;
                break;
            }
        }

        return canPathfindToTile;
    }

    private bool IsAllForegroundPickupsInPath()
    {
        var foregroundPickupNotInPath = false;

        Board.Instance.ForEachPickup(pickup =>
        {
            if (!foregroundPickupNotInPath && pickup.IsForeground)
            {
                foregroundPickupNotInPath |= !this.Path.Contains(pickup.CurrentTile);
            }
        });

        return !foregroundPickupNotInPath;
    }
}
