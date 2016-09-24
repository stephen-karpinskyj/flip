using UnityEngine;

public static class MathUtility
{
    /// <remarks>Based on: http://forum.unity3d.com/threads/how-do-i-find-the-closest-point-on-a-line.340058/</remarks>
    public static Vector2 NearestPointOnLineSegment(Vector2 start, Vector2 end, Vector2 point)
    {
        var line = (end - start);
        var len = line.magnitude;
        line.Normalize();

        var v = point - start;
        var d = Vector3.Dot(v, line);
        d = Mathf.Clamp(d, 0f, len);
        return start + line * d;
    }

    /// <remarks>Based on: http://forum.unity3d.com/threads/moving-an-object-on-a-specific-arc.387757/</remarks>
    public static float CircleArcDistanceToAngleOffset(float distance, float circleRadius)
    {
        return (distance / circleRadius) * Mathf.Rad2Deg;
    }

    /// <remarks>Source: http://answers.unity3d.com/questions/823090/equivalent-of-degree-to-vector2-in-unity.html#answer-823216</remarks>
    public static Vector2 ToHeading(float degrees)
    {
        return Quaternion.AngleAxis(degrees, Vector3.forward) * Vector2.up;
    }

    public static float PercentBetween(float min, float max, float value)
    {
        return Mathf.Clamp01((value - min) / (max - min));
    }
}
