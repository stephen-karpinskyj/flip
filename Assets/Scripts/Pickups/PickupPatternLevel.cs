using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PickupPatternLevel
{
    [SerializeField]
    private int minNoteValue = -1;

    [SerializeField]
    private List<PickupPattern> patterns;

    public int MinNoteValue
    {
        get { return this.minNoteValue; }
    }

    public IEnumerable<PickupPattern> Patterns
    {
        get { return this.patterns; }
    }
}
