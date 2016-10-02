using UnityEngine;

namespace DG.Tweening
{
    public static class TweeningExtensions
    {
        public static void DOPunchPosition(this Transform transform, PunchTweenerData data)
        {
            transform.DOPunchPosition(data.punch, data.duration, data.vibrato, data.elasticity);
        }

        public static void DOPunchRotation(this Transform transform, PunchTweenerData data)
        {
            transform.DOPunchRotation(data.punch, data.duration, data.vibrato, data.elasticity);
        }

        public static void DOPunchScale(this Transform transform, PunchTweenerData data)
        {
            transform.DOPunchScale(data.punch, data.duration, data.vibrato, data.elasticity);
        }
    }
}
