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

    private Vector2 bottomLeft;

    private Tile[,] tiles;

    protected override void Awake()
    {
        base.Awake();

        instance = this;

        this.InitialiseTiles();
    }

    private void Start()
    {
        GameObjectUtility.InstantiatePrefab(this.travellerPrefab, this.transform);
    }

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

                var position = new Vector2(x + this.bottomLeft.x, y + this.bottomLeft.y);
                tile.SetPosition(position);
            }
        }

        Camera.main.orthographicSize = this.dimensions.x + 0.2f;
    }

    public List<Tile> Pathfind(Tile a, Tile b)
    {
        var path = new List<Tile> { a };

        var curr = a;

        var bCoords = this.tiles.CoordinatesOf(b);

        while (curr != b)
        {
            var currCoords = this.GetCoordinates(curr);

            if (currCoords.x != bCoords.x)
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
        var boardPosition = worldPosition - (Vector3)this.bottomLeft;
        var coords = new Vector2I(Mathf.RoundToInt(boardPosition.x), Mathf.RoundToInt(boardPosition.y));

        return this.GetTile(coords);
    }

    public Vector2I GetCoordinates(Tile tile)
    {
        return this.tiles.CoordinatesOf(tile);
    }

    public Tile GetTile(Vector2I coords)
    {
        if (coords.x < 0 || coords.x >= this.tiles.GetLength(0))
        {
            return null;
        }

        if (coords.y < 0 || coords.y >= this.tiles.GetLength(1))
        {
            return null;
        }
        
        return this.tiles[coords.x, coords.y];
    }

    public void ForEachTile(Action<Tile> action)
    {
        foreach (var tile in this.tiles)
        {
            action(tile);
        }
    }
}
