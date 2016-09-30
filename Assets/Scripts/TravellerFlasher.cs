using UnityEngine;
using System.Collections;

public class TravellerFlasher : BaseMonoBehaviour
{
    [SerializeField]
    private Renderer rend;

    [SerializeField]
    private Color flashingColor = Color.black;

    [SerializeField]
    private float flashDuration = 0.1f;

    private Color defaultColor;

    private bool isFlashing;

    protected override void Awake()
    {
        base.Awake();

        this.defaultColor = this.rend.material.color;
    }

    private IEnumerator FlashCoroutine()
    {
        this.ShowFlash(true);

        yield return new WaitForSeconds(this.flashDuration);

        if (this.isFlashing)
        {
            this.ShowFlash(false);
        }
    }

    private void ShowFlash(bool flash)
    {
        this.isFlashing = flash;
        this.rend.material.color = flash ? this.flashingColor : this.defaultColor;
    }

    public void Flash()
    {
        this.StartCoroutine(this.FlashCoroutine());
    }
}
