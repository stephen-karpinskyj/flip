using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SongSection
{
    [SerializeField]
    private List<SongAction> onStart;

    public TrackType FindFocusedTrack()
    {
        var track = TrackType.None;
        
        foreach (var action in this.onStart)
        {
            if (action.Type == SongActionType.FocusTrack)
            {
                track = action.Track;
            }
        }

        return track;
    }

    public void Start(SongPlayer song)
    {
        foreach (var action in this.onStart)
        {
            action.Execute(song);
        }
    }
}
