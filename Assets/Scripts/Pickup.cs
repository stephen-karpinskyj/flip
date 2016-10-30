using UnityEngine;
using DG.Tweening;

public class Pickup : BaseMonoBehaviour
{
    [SerializeField]
    private float moveDuration = 0.05f;

    [SerializeField]
    private float scaleDownDuration = 0.05f;

    [SerializeField]
    private PunchTweenerData punchTweenerData;

    private Tile currentTile;

    private Vector3 defaultScale;

    public delegate void DestroyCallback(Pickup pickup);
    private DestroyCallback onDestroy;

    public Tile CurrentTile
    {
        get { return this.currentTile; }
    }

    private void Awake()
    {
        this.defaultScale = this.transform.localScale;
    }

    private void Start()
    {
        MusicManager.Instance.OnTrackNoteEvent += this.HandleTrackNoteEvent;
        Traveller.Instance.OnStep += this.HandleTravellerStep;
    }

    private void OnDestroy()
    {
        if (MusicManager.Exists)
        {
            MusicManager.Instance.OnTrackNoteEvent -= this.HandleTrackNoteEvent;
        }

        if (Traveller.Exists)
        {
            Traveller.Instance.OnStep -= this.HandleTravellerStep;
        }

        if (this.onDestroy != null)
        {
            this.onDestroy(this);
        }
    }

    public void Initialise(Tile tile, DestroyCallback onDestroy = null)
    {
        this.ChangeTile(tile);
        this.UpdatePosition();

        this.Punch();

        this.onDestroy = onDestroy;
    }

    private void ChangeTile(Tile tile)
    {
        this.currentTile = tile;
    }

    private void StepMovement()
    {
        var tween = this.transform.DOMove(this.currentTile.transform.position, this.moveDuration);
        tween.OnComplete(() => this.UpdatePosition());
    }

    private void UpdatePosition()
    {
        this.transform.position = this.currentTile.transform.position;
    }

    private void Punch()
    {
        this.transform.DOPunchScale(this.punchTweenerData, this.defaultScale);
    }

    private void HandleTrackNoteEvent(string track, int value)
    {
        if (value >= 0)
        {
            this.Punch();
        }
    }

    private void HandleTravellerStep(Tile tile)
    {
        if (tile == this.currentTile)
        {
            var tweener = this.transform.DOScale(Vector3.zero, this.scaleDownDuration);
            tweener.OnComplete(() => Object.Destroy(this.gameObject));
        }
    }
}
