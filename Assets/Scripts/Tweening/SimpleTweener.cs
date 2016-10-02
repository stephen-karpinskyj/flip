using UnityEngine;
using DG.Tweening;

public abstract class SimpleTweener<T> : BaseMonoBehaviour
{
    [SerializeField]
    private float inDuration = 0.05f;

    [SerializeField]
    private float outDuration = 0.15f;

    [SerializeField]
    private T target;

    private T initialValue;

    private bool isPlaying;

    protected virtual void Awake()
    {
        this.initialValue = this.GetInitialValue();
    }

    private void Play(bool playIn, float delay, float duration, TweenCallback onComplete)
    {
        var to = playIn ? this.target : this.initialValue;

        var tween = this.StartTween(to, duration);
        tween.OnComplete(onComplete);
        if (delay > 0f)
        {
            tween.SetDelay(delay);
        }
    }

    public void PlayInOut()
    {
        // TODO: Cancel/restart rather than return
        if (this.isPlaying)
        {
            return;
        }

        this.Play(true, 0f, this.inDuration,
            () => this.Play(false, 0f, this.outDuration,
                () => this.isPlaying = false));
    }

    protected abstract T GetInitialValue();
    protected abstract Tweener StartTween(T target, float duration);
}
