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

    private Tile highwayCreationStartTile;

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

    public TilePath CurrentPath
    {
        get { return this.pathDrawer.Path; }
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

        if (nextTile != null)
        {
            var isStillStepping = true;
            var hasPickup = MusicManager.Instance.Player.HasPickup(nextTile);

            // Skip over highways
            if (!hasPickup)
            {
                if (this.currentTile.CurrentHighwayMode != Tile.HighwayMode.Is || nextTile.CurrentHighwayMode != Tile.HighwayMode.Is)
                {
                    isStillStepping = false;
                }

                while (isStillStepping)
                {
                    var peekedTile = this.pathDrawer.Path.PeekFirstTile();
                    hasPickup = MusicManager.Instance.Player.HasPickup(peekedTile);

                    if (hasPickup)
                    {
                        var isInTravellingDirection = peekedTile.Coordinates.SharesAxisWith(this.CurrentTile.Coordinates, nextTile.Coordinates);

                        if (isInTravellingDirection)
                        {
                            nextTile = this.pathDrawer.Path.PopFirstTile();
                        }

                        isStillStepping = false;
                    }
                    else
                    {
                        if (peekedTile == null)
                        {
                            isStillStepping = false;
                        }
                        else
                        {
                            var isInTravellingDirection = peekedTile.Coordinates.SharesAxisWith(this.CurrentTile.Coordinates, nextTile.Coordinates);

                            if (!isInTravellingDirection || peekedTile.CurrentHighwayMode != Tile.HighwayMode.Is)
                            {
                                isStillStepping = false;
                            }
                            else
                            {
                                nextTile = this.pathDrawer.Path.PopFirstTile();
                            }
                        }
                    }
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

        this.OnStep(this.currentTile);
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
