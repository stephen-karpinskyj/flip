using System.Collections.Generic;
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

    private bool StepMovement()
    {
        var nextTile = this.pathDrawer.Path.PopFirstTile();

        if (nextTile == null)
        {
            return false;
        }

        var isStillStepping = true;

        // Check for pickup
        {
            var hasPickup = MusicManager.Instance.Player.HasPickup(nextTile);

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

                var hasPickup = MusicManager.Instance.Player.HasPickup(peekedTile);

                if (hasPickup)
                {
                    while (nextTile != peekedTile)
                    {
                        nextTile = this.pathDrawer.Path.PopFirstTile();
                    }

                    isStillStepping = false;
                    break;
                }

                peekedTile = this.pathDrawer.Path.PeekNextTile(peekedTile);
            }
        }

        // Skip over highways
        if (isStillStepping)
        {
            if (!this.currentTile.IsHighway || !nextTile.IsHighway)
            {
                isStillStepping = false;
            }

            while (isStillStepping)
            {
                var peekedTile = this.pathDrawer.Path.PeekFirstTile();

                if (peekedTile == null)
                {
                    isStillStepping = false;
                    continue;
                }

                var isInTravellingDirection = peekedTile.Coordinates.SharesAxisWith(this.CurrentTile.Coordinates, nextTile.Coordinates);

                if (!isInTravellingDirection || !peekedTile.IsHighway)
                {
                    isStillStepping = false;
                    continue;
                }

                nextTile = this.pathDrawer.Path.PopFirstTile();
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

        this.OnStep(this.currentTile);

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
        this.pathDrawer.StartingDirection = this.currentDir;
    }

    private void HandleTrackNoteEvent(TrackType track, int value)
    {
        this.StepMovement();
        this.transform.DOPunchScale(this.punchTweenerData, this.defaultScale);
    }
}
