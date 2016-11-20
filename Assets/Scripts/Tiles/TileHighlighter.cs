using UnityEngine;

public class TileHighlighter : BaseMonoBehaviour
{
    [SerializeField]
    private Renderer rend;

    private Color32 defaultColor;

    public bool IsHighlighted { get; private set; }

    private void Awake()
    {
        this.defaultColor = this.rend.material.color;
    }

    public void Highlight(float colourScale, bool highlight)
    {
        if (highlight == this.IsHighlighted)
        {
            return;
        }

        var color = this.defaultColor;

        if (highlight)
        {
            color.r = (byte)(color.r * colourScale);
            color.g = (byte)(color.g * colourScale);
//            color.b = (byte)(color.b * colourScale);
        }

        this.IsHighlighted = highlight;
        this.rend.material.color = color;
    }
}
