using System;
using UnityEngine;

[Serializable]
public struct BoardDirection
{
    public Vector2 Value { get; private set; }

    public void Set(Tile from, Tile to)
    {
        if (from == null || to == null)
        {
            return;
        }

        var fromC = from.Coordinates;
        var toC = to.Coordinates;

        if (fromC.x != toC.x)
        {
            this.SetValue(Mathf.RoundToInt(Mathf.Sign(toC.x - fromC.x)), 0);
        }
        else if (fromC.y != toC.y)
        {
            this.SetValue(0, Mathf.RoundToInt(Mathf.Sign(toC.y - fromC.y)));
        }
    }

    public void SetDefault()
    {
        this.SetValue(0, 1);
    }

    public static BoardDirection Default()
    {
        var dir = new BoardDirection();
        dir.SetDefault();
        return dir;
    }

    private void SetValue(int x, int y)
    {
        var newValue = this.Value;
        newValue.Set(x, y);
        this.Value = newValue;
    }
}
