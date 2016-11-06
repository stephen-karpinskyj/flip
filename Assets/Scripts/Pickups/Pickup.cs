using UnityEngine;
using DG.Tweening;

public abstract class Pickup : BaseMonoBehaviour
{
    private const float BackgroundTransparency = 0.3f;

    [SerializeField]
    private float scaleDownDuration = 0.075f;

    [SerializeField, Range(0f, 1f)]
    private float backgroundScaleMultiplier = 0.75f;

    [SerializeField]
    private Renderer outerBackingRenderer;

    [SerializeField]
    private PunchTweenerData punchTweenerData;

    private PickupLayer currentLayer;

    private Tile currentTile;

    private Vector3 defaultScale;
    private float defaultTransparency;

    public delegate void DestroyCallback(Pickup pickup);
    private DestroyCallback onDestroy;

    public PickupLayer CurrentLayer
    {
        get { return this.currentLayer; }
    }

    public Tile CurrentTile
    {
        get { return this.currentTile; }
    }

    protected virtual void Awake()
    {
        this.currentLayer = PickupLayer.Foreground;

        this.defaultScale = this.transform.localScale;
        this.defaultTransparency = this.outerBackingRenderer.material.color.a;
    }

    protected virtual void Start()
    {
        MusicManager.Instance.Player.OnTrackNoteEvent += this.HandleTrackNoteEvent;
        Traveller.Instance.OnStep += this.HandleTravellerStep;
    }

    protected virtual void OnDestroy()
    {
        if (MusicManager.Exists)
        {
            MusicManager.Instance.Player.OnTrackNoteEvent -= this.HandleTrackNoteEvent;
        }

        if (Traveller.Exists)
        {
            Traveller.Instance.OnStep -= this.HandleTravellerStep;
        }
    }

    public void Initialise(Tile tile, PickupLayer layer, DestroyCallback onDestroy = null)
    {
        this.currentTile = tile;
        this.transform.position = this.currentTile.transform.position;

        this.SetLayer(layer);

        this.onDestroy = onDestroy;
    }

    public void SetLayer(PickupLayer layer)
    {
        this.currentLayer = layer;

        var color = this.outerBackingRenderer.material.color;

        if (this.currentLayer == PickupLayer.Background)
        {
            color.a = BackgroundTransparency;
        }
        else if (this.currentLayer == PickupLayer.Foreground)
        {
            color.a = this.defaultTransparency;
        }

        this.outerBackingRenderer.material.color = color;

        this.Punch();
    }

    private void Punch()
    {
        var scale = defaultScale;

        if (this.currentLayer == PickupLayer.Background)
        {
            scale *= this.backgroundScaleMultiplier;
        }

        this.transform.DOPunchScale(this.punchTweenerData, scale);
    }

    private void ScaleDown(TweenCallback onComplete)
    {
        var tweener = this.transform.DOScale(Vector3.zero, this.scaleDownDuration);
        tweener.OnComplete(onComplete);
    }

    private void HandleTrackNoteEvent(TrackType track, int value)
    {
        if (this.currentLayer == PickupLayer.Background || value < 0)
        {
            return;
        }

        this.Punch();
    }

    private void HandleTravellerStep(Tile tile)
    {
        if (this.currentLayer == PickupLayer.Background || tile != this.currentTile)
        {
            return;
        }

        this.OnPickup();

        this.ScaleDown(delegate
        {
            Object.Destroy(this.gameObject);
        });

        if (this.onDestroy != null)
        {
            this.onDestroy(this);
        }
    }

    protected virtual void OnPickup() { }
}
