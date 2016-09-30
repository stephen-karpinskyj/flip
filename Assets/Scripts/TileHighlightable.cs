using UnityEngine;

public class TileHighlightable : BaseMonoBehaviour
{
    [SerializeField]
    private Renderer rend;

    [SerializeField]
    private Color highlightedColor = Color.black;

    private Color defaultColor;

    private bool isHighlighted;

    protected override void Awake()
    {
        base.Awake();

        this.defaultColor = this.rend.material.color;
    }

    public void Highlight(bool highlight)
    {
        if (highlight == this.isHighlighted)
        {
            return;
        }

        this.isHighlighted = highlight;
        this.rend.material.color = highlight ? this.highlightedColor : this.defaultColor;
    }
}
