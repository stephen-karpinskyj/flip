using UnityEngine;

public class Board : BaseMonoBehaviour
{
    [SerializeField]
    private Vector2I dimensions = new Vector2I(5, 9);

    [SerializeField]
    private Tile tilePrefab;

    private Tile[,] tiles;

    protected override void Awake()
    {
        base.Awake();

        this.InitialiseTiles();
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
            }
        }
    }
}
