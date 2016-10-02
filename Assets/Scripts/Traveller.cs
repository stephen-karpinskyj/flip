using UnityEngine;
using DG.Tweening;

public class Traveller : BaseMonoBehaviour
{
    [SerializeField]
    private float moveDuration = 0.05f;

    [SerializeField]
    private Vector2I initialCoords;

    [SerializeField]
    private ColorTweener centreColourTweener;

    [SerializeField]
    private PunchTweenerData punchTweenerData;

    private PathDrawer pathDrawer = new PathDrawer();

    private Tile currentTile;
    private BoardDirection currentDir;

    private void Start()
    {   
        var initialTile = Board.Instance.GetTile(this.initialCoords);
        this.ChangeTile(initialTile);

        this.UpdatePosition();

        MusicManager.Instance.OnDrumsEvent += koreoEvent =>
        {
            //this.centreColourTweener.PlayInOut();
            this.StepMovement();
            this.transform.DOPunchScale(this.punchTweenerData);
        };
    }

    private void Update()
    {
        this.pathDrawer.CheckInput();
    }

    private void UpdatePosition()
    {
        this.transform.position = this.currentTile.transform.position;
    }

    private void UpdateRotation()
    {
        this.transform.eulerAngles = this.CalculateRotation();
    }

    private Vector3 CalculateRotation()
    {
        var angle = BoardDirection.Default().Value.SignedAngle(this.currentDir.Value);
        return Vector3.forward * angle;
    }

    // TODO: If no path, move in non-180 to last move
    // TODO: If path, move in non-180 from last tile (if not in same axis)

    private bool StepMovement()
    {
        var nextTile = this.pathDrawer.Path.PopFirstTile();

        if (nextTile == null)
        {
            return false;
        }

        this.ChangeTile(nextTile);

        this.transform.DORotate(this.CalculateRotation(), this.moveDuration);
        var tween = this.transform.DOMove(nextTile.transform.position, this.moveDuration);
        tween.OnComplete(() =>
        {
            this.UpdatePosition();
            this.UpdateRotation();
        });

        return true;
    }

    private void ChangeTile(Tile tile)
    {
        if (this.currentTile == null)
        {
            this.currentDir.SetDefault();
        }
        else if (this.currentTile == tile)
        {
            this.currentDir.Set(this.currentTile, this.pathDrawer.Path.PeekFirstTile());
        }
        else
        {
            this.currentDir.Set(this.currentTile, tile);
        }

        this.currentTile = tile;
        this.pathDrawer.AllowedStartingTile = tile;
    }
}
