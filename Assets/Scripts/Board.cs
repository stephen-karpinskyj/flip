using UnityEngine;
using System.Collections;

public class Board : BaseMonoBehaviour
{
    [SerializeField]
    private Vector2I dimensions = new Vector2I(5, 9);

    [SerializeField]
    private Tile tilePrefab;

    [SerializeField]
    private Face facePrefab;

    private Tile[,] tiles;

    protected override void Awake()
    {
        base.Awake();

        Debug.Assert(this.tilePrefab, this);
        Debug.Assert(this.facePrefab, this);

        this.InitialiseTiles();
    }

    private IEnumerator Start()
    {
        // TEMP
        while (true)
        {
            var randomTile = this.tiles
            [
                Random.Range(0, this.tiles.GetLength(0)),
                Random.Range(0, this.tiles.GetLength(1))
            ];

            randomTile.Flip(false);

            yield return new WaitForSeconds(0.01f);
        }
    }

    private void InitialiseTiles()
    {
        this.tiles = new Tile[this.dimensions.x, this.dimensions.y];

        var originOffset = this.dimensions / -2;
            
        for (var x = 0; x < this.dimensions.x; x++)
        {
            for (var y = 0; y < this.dimensions.y; y++)
            {
                var tile = GameObjectUtility.InstantiatePrefab(this.tilePrefab, this.transform);
                tile.name = string.Format("Tile {0}:{1}", x, y);

                this.tiles[x, y] = tile;

                var position = new Vector2(x + originOffset.x, y + originOffset.y);
                tile.SetPosition(position);

                for (var i = 0; i < 2; i++)
                {
                    var face = GameObjectUtility.InstantiatePrefab(this.facePrefab);
                    tile.AddFace(face);
                }
            }
        }
    }
}
