using System;
using UnityEngine;

[Serializable]
public class PickupPattern
{
    [SerializeField]
    private PickupType type;

    [SerializeField]
    private Vector2I position;

    public PickupType Type
    {
        get { return this.type; }
    }

    public Vector2I Coordinates
    {
        get { return this.position; }
    }
}
