using System;
using UnityEngine;

[Serializable]
public struct BoardDirection
{
    public Vector2 Value { get; private set; }

    /// <remarks>Simple, intended for testing adjacent tiles.</remarks>
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

    /// <remarks>More complex, intended for testing non-adjacent tiles.</remarks>
    public void Set(Tile from, Tile to, BoardDirection fromDirection)
    {
        if (from == null || to == null)
        {
            return;
        }

        var fromC = from.Coordinates;
        var toC = to.Coordinates;

        var isGoingBackwardsX = false;
        var xDir = Mathf.Sign(toC.x - fromC.x);
        var facingX = !Mathf.Approximately(fromDirection.Value.x, 0f);

        if (!Mathf.Approximately(xDir, 0f) && facingX)
        {
            isGoingBackwardsX = !Mathf.Approximately(Mathf.Sign(xDir), Mathf.Sign(fromDirection.Value.x));
        }

        var isGoingBackwardsY = false;
        var yDir = Mathf.Sign(toC.y - fromC.y);
        var facingY = !Mathf.Approximately(fromDirection.Value.y, 0f);

        if (!Mathf.Approximately(yDir, 0f) && facingY)
        {
            isGoingBackwardsY = !Mathf.Approximately(Mathf.Sign(yDir), Mathf.Sign(fromDirection.Value.y));
        }

        if (facingX)
        {
            if (isGoingBackwardsX)
            {
                this.SetValue(0, Mathf.RoundToInt(yDir));
            }
            else
            {
                this.SetValue(Mathf.RoundToInt(xDir), 0);
            }
        }
        else if (facingY)
        {
            if (isGoingBackwardsY)
            {
                this.SetValue(Mathf.RoundToInt(xDir), 0);
            }
            else
            {
                this.SetValue(0, Mathf.RoundToInt(yDir));
            }
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
