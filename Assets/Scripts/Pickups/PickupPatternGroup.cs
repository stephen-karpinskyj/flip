using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PickupPatternGroup
{
    [SerializeField]
    private TrackType track;

    [SerializeField]
    private List<PickupPatternLevel> levels;

    public TrackType Track
    {
        get { return this.track; }
    }

    public PickupPatternLevel GetPatterns(int level)
    {
        if (level < 0 || level >= this.levels.Count)
        {
            return null;
        }

        return this.levels[level];
    }
}
