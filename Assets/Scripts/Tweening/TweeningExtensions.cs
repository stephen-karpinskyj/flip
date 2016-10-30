using UnityEngine;

namespace DG.Tweening
{
    public static class TweeningExtensions
    {
        public static Tweener DOPunchPosition(this Transform transform, PunchTweenerData data)
        {
            return transform.DOPunchPosition(data.punch, data.duration, data.vibrato, data.elasticity);
        }

        public static Tweener DOPunchRotation(this Transform transform, PunchTweenerData data)
        {
            return transform.DOPunchRotation(data.punch, data.duration, data.vibrato, data.elasticity);
        }

        public static Tweener DOPunchScale(this Transform transform, PunchTweenerData data, Vector3 defaultScale)
        {
            transform.localScale = defaultScale;
            var tweener = transform.DOPunchScale(data.punch, data.duration, data.vibrato, data.elasticity);
            tweener.OnComplete(() => transform.localScale = defaultScale);

            return tweener;
        }
    }
}
