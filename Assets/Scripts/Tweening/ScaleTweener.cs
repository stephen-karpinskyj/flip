using UnityEngine;
using DG.Tweening;

public class ScaleTweener : SimpleTweener<Vector3>
{
    [SerializeField]
    private Transform trans;

    protected override void Awake()
    {
        base.Awake();
        
        Debug.Assert(this.trans, this);
    }

    protected override Vector3 GetInitialValue()
    {
        return this.trans.localScale;
    }

    protected override Tweener StartTween(Vector3 target, float duration)
    {
        return this.trans.DOScale(target, duration);
    }
}
