using UnityEngine;
using DG.Tweening;

public class ColorTweener : SimpleTweener<Color>
{ 
    [SerializeField]
    private Renderer rend;

    protected override void Awake()
    {
        base.Awake();

        Debug.Assert(this.rend, this);
    }

    protected override Color GetInitialValue()
    {
        return this.rend.material.color;
    }

    protected override Tweener StartTween(Color target, float duration)
    {
        return this.rend.material.DOColor(target, duration);
    }
}
