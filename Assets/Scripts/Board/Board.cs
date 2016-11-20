using System;
using System.Collections.Generic;
using UnityEngine;

public class Board : BehaviourSingleton<Board>
{
    [SerializeField]
    private Vector2I dimensions = new Vector2I(5, 9);

    [SerializeField]
    private Tile tilePrefab;

    [SerializeField]
    private Traveller travellerPrefab;

    [SerializeField]
    private List<PickupPrefab> pickupPrefabs;

    private Vector2 bottomLeft;

    private Tile[,] tiles;

    private List<Pickup> currentPickups;

    public delegate void PickupDestroyCallback(Pickup pickup);
    public event PickupDestroyCallback OnPickupDestroy = delegate { };

    private void Awake()
    {
        this.InitialiseTiles();
        this.InitialisePickups();
    }


    #region Tiles


    private void InitialiseTiles()
    {
        this.tiles = new Tile[this.dimensions.x, this.dimensions.y];

        this.bottomLeft = new Vector2(this.dimensions.x, this.dimensions.y) / -2;
        this.bottomLeft.x += 0.5f;
        this.bottomLeft.y += 0.5f;

        for (var x = 0; x < this.dimensions.x; x++)
        {
            for (var y = 0; y < this.dimensions.y; y++)
            {
                var tile = GameObjectUtility.InstantiatePrefab(this.tilePrefab, this.transform);
                tile.name = string.Format("Tile {0}:{1}", x, y);

                this.tiles[x, y] = tile;

                tile.Coordinates = new Vector2I(x, y);

                var position = new Vector2(x + this.bottomLeft.x, y + this.bottomLeft.y);
                tile.SetPosition(position);
            }
        }
    }

    public List<Tile> Pathfind(Tile a, Tile b, BoardDirection inDirection)
    {
        var path = new List<Tile> { a };

        var bCoords = b.Coordinates;
        var aCoords = a.Coordinates;

        var xFirst = !Mathf.Approximately(inDirection.Value.x, 0);

        var curr = a;

        while (curr != b)
        {
            var currCoords = curr.Coordinates;

            if (currCoords.x == bCoords.x)
            {
                xFirst = false;
            }
            else
            {
                xFirst |= currCoords.y == bCoords.y;
            }

            if (xFirst && currCoords.x != bCoords.x)
            {
                currCoords.x += Mathf.RoundToInt(Mathf.Sign(bCoords.x - currCoords.x));
            }
            else if (currCoords.y != bCoords.y)
            {
                currCoords.y += Mathf.RoundToInt(Mathf.Sign(bCoords.y - currCoords.y));
            }

            curr = this.GetTile(currCoords);
            path.Add(curr);
        }

        return path;
    }

    public Tile GetTile(Vector2 screenPosition)
    {
        var worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        var boardPosition = worldPosition - (Vector3)this.bottomLeft - this.transform.position;
        var coords = new Vector2I(Mathf.RoundToInt(boardPosition.x), Mathf.RoundToInt(boardPosition.y));

        return this.GetTile(coords);
    }

    public Tile GetTile(Vector2I coords)
    {
        coords.x = Mathf.Clamp(coords.x, 0, this.tiles.GetLength(0) - 1);
        coords.y = Mathf.Clamp(coords.y, 0, this.tiles.GetLength(1) - 1);
        
        return this.tiles[coords.x, coords.y];
    }

    public Tile GetAdjacentTile(Tile tile, BoardDirection inDirection)
    {
        if (tile == null)
        {
            return null;
        }

        var adjacentCoords = tile.Coordinates;

        if (!Mathf.Approximately(inDirection.Value.x, 0f))
        {
            adjacentCoords.x += inDirection.Value.x > 0 ? 1 : -1;
        }
        else if (!Mathf.Approximately(inDirection.Value.y, 0f))
        {
            adjacentCoords.y += inDirection.Value.y > 0 ? 1 : -1;
        }

        return this.GetTile(adjacentCoords);
    }

    public void ForEachTile(Action<Tile> action)
    {
        foreach (var tile in this.tiles)
        {
            action(tile);
        }
    }


    #endregion


    #region Pickups


    private void InitialisePickups()
    {
        this.currentPickups = new List<Pickup>();
    }

    private Pickup SpawnPickup(PickupPattern pattern, bool isForeground)
    {
        var pickup = this.pickupPrefabs.Find(p => p.Type == pattern.Type).Instantiate(this.transform);
        this.currentPickups.Add(pickup);

        var tile = Board.Instance.GetTile(pattern.Coordinates);
        pickup.Initialise(tile, isForeground, this.HandlePickupDestroy);

        return pickup;
    }

    public bool HasPickup(Tile tile, bool includeBackground)
    {
        if (tile == null)
        {
            return false;
        }

        var pickup = this.currentPickups.Find(p => p.CurrentTile == tile);

        if (pickup == null)
        {
            return false;
        }

        if (!includeBackground && !pickup.IsForeground)
        {
            return false;
        }

        return true;
    }

    public bool HasPickup(bool includeBackground)
    {
        if (includeBackground)
        {
            return this.currentPickups.Count > 0;
        }

        return this.currentPickups.Find(p => p.IsForeground) != null;
    }

    public Pickup AddForegroundPickup(PickupPattern pattern)
    {
        var pickup = this.currentPickups.Find(p => p.CurrentTile.Coordinates == pattern.Coordinates);

        if (pickup == null)
        {
            pickup = this.SpawnPickup(pattern, true);
            pickup.Hide(false);

            return pickup;
        }

        if (!pickup.IsForeground)
        {
            pickup.SetForeground(true);
            pickup.Hide(false);

            return pickup;
        }

        return pickup;
    }

    public Pickup AddBackgroundPickup(PickupPattern pattern)
    {
        var pickup = this.currentPickups.Find(p => p.CurrentTile.Coordinates == pattern.Coordinates);

        if (pickup == null)
        {
            pickup = this.SpawnPickup(pattern, false);
            pickup.Hide(true);

            return pickup;
        }

        if (pickup.IsForeground)
        {
            return null;
        }

        return pickup;
    }

    public void ForEachPickup(Action<Pickup> action)
    {
        foreach (var pickup in this.currentPickups)
        {
            action(pickup);
        }
    }

    private void HandlePickupDestroy(Pickup pickup)
    {
        this.currentPickups.Remove(pickup);

        this.OnPickupDestroy(pickup);
    }


    #endregion
}
