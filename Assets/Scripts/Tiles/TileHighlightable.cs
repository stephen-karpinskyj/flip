using UnityEngine;

public class TileHighlightable : BaseMonoBehaviour
{
    [SerializeField]
    private Renderer rend;

    [SerializeField]
    private Color highlightedColor = Color.black;

    private Color defaultColor;

    public bool IsHighlighted { get; private set; }

    private void Awake()
    {
        this.defaultColor = this.rend.material.color;
    }

    public void Highlight(bool highlight)
    {
        if (highlight == this.IsHighlighted)
        {
            return;
        }

        this.IsHighlighted = highlight;
        this.rend.material.color = highlight ? this.highlightedColor : this.defaultColor;
    }
}
