using UnityEngine;
using DG.Tweening;

public class Traveller : BehaviourSingleton<Traveller>
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

    private Vector3 defaultScale;

    public delegate void StepCallback(Tile tile);
    public StepCallback OnStep = delegate { };

    public Tile CurrentTile
    {
        get { return this.currentTile; }
    }

    public BoardDirection CurrentDirection
    {
        get { return this.currentDir; }
    }

    public PathDrawer PathDrawer
    {
        get { return this.pathDrawer; }
    }

    private void Awake()
    {
        this.defaultScale = this.transform.localScale;
    }

    private void Start()
    {   
        var initialTile = Board.Instance.GetTile(this.initialCoords);
        this.ChangeTile(initialTile);

        this.UpdatePosition();

        MusicManager.Instance.Player.OnTrackNoteEvent += this.HandleTrackNoteEvent;
    }

    private void OnDestroy()
    {
        if (MusicManager.Exists)
        {
            MusicManager.Instance.Player.OnTrackNoteEvent -= this.HandleTrackNoteEvent;
        }
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

    private void StepMovement()
    {
        var nextTile = this.pathDrawer.Path.PopFirstTile();

        var isStillStepping = true;

        if (nextTile != null)
        {
            // Check for pickup
            {
                var hasPickup = Board.Instance.HasPickup(nextTile, false);

                if (hasPickup)
                {
                    isStillStepping = false;
                }
            }

            // Jump to next pickup
            if (isStillStepping)
            {
                var peekedTile = this.pathDrawer.Path.PeekFirstTile();

                while (peekedTile != null)
                {
                    var isInTravellingDirection = peekedTile.Coordinates.SharesAxisWith(this.CurrentTile.Coordinates, nextTile.Coordinates);

                    if (!isInTravellingDirection)
                    {
                        break;
                    }

                    var hasPickup = Board.Instance.HasPickup(peekedTile, false);

                    if (hasPickup)
                    {
                        while (nextTile != peekedTile)
                        {
                            nextTile = this.pathDrawer.Path.PopFirstTile();
                        }

                        break;
                    }

                    peekedTile = this.pathDrawer.Path.PeekNextTile(peekedTile);
                }
            }

            this.ChangeTile(nextTile);

            this.transform.DORotate(this.CalculateRotation(), this.moveDuration);
            var tween = this.transform.DOMove(nextTile.transform.position, this.moveDuration);
            tween.OnComplete(() =>
            {
                this.UpdatePosition();
                this.UpdateRotation();
            });
        }

        this.OnStep(nextTile ?? this.currentTile);
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
        this.pathDrawer.StartingDirection = this.currentDir;
    }

    private void HandleTrackNoteEvent(TrackType track, int value)
    {
        this.StepMovement();
        this.transform.DOPunchScale(this.punchTweenerData, this.defaultScale);
    }
}
