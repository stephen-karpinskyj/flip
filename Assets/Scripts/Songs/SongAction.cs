using System;
using UnityEngine;

[Serializable]
public class SongAction
{
    [SerializeField]
    private SongActionType type;

    [SerializeField]
    private TrackType track;

    public void Execute(SongPlayer player)
    {
        switch (this.type)
        {
            case SongActionType.StartTrack: player.StartTrack(this.track, true); break;
            case SongActionType.StopTrack: player.StartTrack(this.track, false); break;
            case SongActionType.FocusTrack: player.FocusTrack(this.track); break;
            case SongActionType.EndSection: player.EndSection(); break;
            default: throw new NotImplementedException();
        }
    }
}
