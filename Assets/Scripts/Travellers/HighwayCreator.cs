public class HighwayCreator : BaseMonoBehaviour
{
    private Tile startTile;
    private Tile endTile;

    private void Start()
    {
        Traveller.Instance.OnStep += this.HandleTravellerStep;
        MusicManager.Instance.Player.OnPickupDestroy += this.HandlePickupDestroy;
    }

    private void OnDestroy()
    {
        if (Traveller.Exists)
        {
            Traveller.Instance.OnStep -= this.HandleTravellerStep;
        }

        if (MusicManager.Exists)
        {
            MusicManager.Instance.Player.OnPickupDestroy -= this.HandlePickupDestroy;
        }
    }

    private void StartHighway(Tile tile)
    {
        this.startTile = tile;

        if (tile.CurrentHighwayMode != Tile.HighwayMode.Is)
        {
            this.startTile.SetHighwayMode(Tile.HighwayMode.MightBe);
        }
    }

    private void CompleteHighway()
    {
        if (this.startTile == null || this.endTile == null)
        {
            return;
        }

        var tiles = Board.Instance.Pathfind(this.startTile, this.endTile, Traveller.Instance.CurrentDirection);

        foreach (var tile in tiles)
        {
            if (tile.CurrentHighwayMode != Tile.HighwayMode.Is)
            {
                tile.SetHighwayMode(Tile.HighwayMode.Is);
            }
        }

        this.startTile = null;
    }

    private void ResetStartedHighway()
    {
        Board.Instance.ForEachTile(tile =>
        {
            if (tile.CurrentHighwayMode != Tile.HighwayMode.Is)
            {
                tile.SetHighwayMode(Tile.HighwayMode.IsNot);
            }
        });

        this.startTile = null;
    }

    private void RefreshStartedHighway()
    {
        if (this.startTile == null || this.endTile == null)
        {
            return;
        }

        var tiles = Board.Instance.Pathfind(this.startTile, this.endTile, Traveller.Instance.CurrentDirection);

        foreach (var tile in tiles)
        {
            if (tile.CurrentHighwayMode == Tile.HighwayMode.IsNot)
            {
                tile.SetHighwayMode(Tile.HighwayMode.MightBe);
            }
        }
    }

    private void ClearAllHighways()
    {
        Board.Instance.ForEachTile(tile =>
        {
            if (tile.CurrentHighwayMode != Tile.HighwayMode.IsNot)
            {
                tile.SetHighwayMode(Tile.HighwayMode.IsNot);
            }
        });
    }

    private void HandleTravellerStep(Tile tile)
    {
        if (this.endTile == tile)
        {
            this.ResetStartedHighway();
            return;
        }

        this.endTile = tile;

        if (this.startTile != null)
        {
            var pathwayIsStraight = this.endTile.Coordinates.SharesAxisWith(this.startTile.Coordinates);

            if (pathwayIsStraight)
            {
                this.RefreshStartedHighway();
            }
            else
            {
                this.ResetStartedHighway();
            }
        }
    }

    private void HandlePickupDestroy(Pickup pickup)
    {
        if (MusicManager.Instance.Player.IsEndingSection)
        {
            this.ClearAllHighways();
            return;
        }
        
        this.CompleteHighway();
        this.ResetStartedHighway();
        this.StartHighway(pickup.CurrentTile);
    }
}
