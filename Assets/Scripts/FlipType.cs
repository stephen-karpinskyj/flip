using UnityEngine;

public enum FlipType
{
    PosX = 0,
    NegX,
    PosY,
    NegY,
}

public static class FlipTypeExtensions
{
    public static Vector2 ToOffset(this FlipType type)
    {
        var offset = Vector2.zero;

        switch (type)
        {
            case FlipType.PosX: offset.x = 1; break;
            case FlipType.NegX: offset.x = -1; break;
            case FlipType.PosY: offset.y = 1; break;
            case FlipType.NegY: offset.y = -1; break;
        }

        return offset * 180;
    }
}