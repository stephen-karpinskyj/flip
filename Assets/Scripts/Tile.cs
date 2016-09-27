using UnityEngine;

public class Tile : BaseMonoBehaviour
{
    [SerializeField]
    private Renderer backingRenderer;

    [SerializeField]
    private Color pressedColor = Color.red;

    private Color defaultColor;
    private bool isPressed;

    protected override void Awake()
    {
        base.Awake();

        this.defaultColor = this.backingRenderer.material.color;
    }

    public void SetPosition(Vector2 position)
    {
        this.transform.localPosition = position;
    }

    public void OnMouseDown()
    {
        this.isPressed = !this.isPressed;
        this.backingRenderer.material.color = this.isPressed ? this.pressedColor : this.defaultColor;
    }
}
