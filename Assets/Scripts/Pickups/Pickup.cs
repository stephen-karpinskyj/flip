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

    [SerializeField]
    private Highlightable backing;

    private bool isHidden;
    private bool isDestroying;
    private bool isPickupable;

    private Tile currentTile;

    private Vector3 defaultScale;
    private float defaultTransparency;

    public delegate void DestroyCallback(Pickup pickup);
    private DestroyCallback onDestroy;

    public bool IsForeground { get; private set; }

    public Tile CurrentTile
    {
        get { return this.currentTile; }
    }

    protected virtual void Awake()
    {
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

    public void Initialise(Tile tile, bool isForeground, DestroyCallback onDestroy = null)
    {
        if (this.isDestroying)
        {
            return;
        }

        this.currentTile = tile;
        this.transform.position = this.currentTile.transform.position;

        this.SetForeground(isForeground);

        this.onDestroy = onDestroy;
    }

    public void SetForeground(bool isForeground)
    {
        if (this.isDestroying)
        {
            return;
        }

        this.IsForeground = isForeground;

        var color = this.outerBackingRenderer.material.color;

        if (this.IsForeground)
        {
            color.a = this.defaultTransparency;
        }
        else
        {
            color.a = BackgroundTransparency;
        }

        this.outerBackingRenderer.material.color = color;

        this.Punch(true);
    }

    public void SetPickupable(bool isPickupable)
    {
        this.isPickupable = isPickupable;

        this.backing.Highlight(this.isPickupable);
    }

    private void Punch(bool force)
    {
        var scale = defaultScale;

        if (!this.IsForeground)
        {
            scale *= this.backgroundScaleMultiplier;
        }

        if (!force && (!this.isPickupable || this.isDestroying))
        {
            this.transform.localScale = scale;
        }
        else
        {
            this.transform.DOPunchScale(this.punchTweenerData, scale);
        }
    }

    public void Hide(bool hide)
    {
        if (this.gameObject.activeSelf == !hide)
        {
            return;
        }
        
        this.gameObject.SetActive(!hide);

        if (!hide)
        {
            this.Punch(true);
        }
    }

    private void ScaleDown(TweenCallback onComplete)
    {
        var tweener = this.transform.DOScale(Vector3.zero, this.scaleDownDuration);
        tweener.OnComplete(onComplete);
    }

    private void HandleTrackNoteEvent(TrackType track, int value)
    {
        if (this.isDestroying || !this.IsForeground)
        {
            return;
        }

        if (value >= 0)
        {
            this.Punch(false);
        }
    }

    private void HandleTravellerStep(Tile tile)
    {
        if (this.isDestroying || !this.IsForeground)
        {
            return;
        }

        if (tile != this.currentTile)
        {
            return;
        }

        this.isDestroying = true;

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
