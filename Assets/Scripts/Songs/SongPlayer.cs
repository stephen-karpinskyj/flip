using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using System;

public class SongPlayer
{
    public delegate void TrackNoteEventCallback(TrackType track, int value);
    public event TrackNoteEventCallback OnTrackNoteEvent = delegate { };

    public delegate void MeasureChangeCallback(int numMeasure);
    public event MeasureChangeCallback OnMeasureChange = delegate { };

    public delegate void ScoreBeatCallback(int numScoreBeats);
    public event ScoreBeatCallback OnScoreBeat = delegate { };

    public delegate void SongEndCallback();
    public event SongEndCallback OnSongEnd = delegate { };

    //TOOD: Move to Board
    public delegate void PickupDestroyCallback(Pickup pickup);
    public event PickupDestroyCallback OnPickupDestroy = delegate { };

    private readonly SongPlayerData data;

    private int currentBeat;
    private int currentScoreBeat;
    private int currentMeasure;
    private int currentSection;
    private TrackType currentFocusedTrack;

    private List<Pickup> currentPickups; // TODO: Move to Board
    private Dictionary<TrackType, int> currentTrackLevels;
    private bool canMoveToNextSection;

    public int CurrentScoreBeat
    {
        get { return this.currentScoreBeat; }
    }

    public int CurrentMeasure
    {
        get { return this.currentMeasure; }
    }

    public bool IsEndingSection
    {
        get { return this.canMoveToNextSection; }
    }

    public SongPlayer(SongPlayerData data)
    {
        this.data = data;

        this.currentBeat = -1;
        this.currentScoreBeat = 0;
        this.currentMeasure = -1;
        this.currentSection = -1;
        this.currentFocusedTrack = TrackType.None;

        this.currentPickups = new List<Pickup>();
        this.currentTrackLevels = new Dictionary<TrackType, int>();
        this.canMoveToNextSection = true;

        this.ListenToEvents();
    }

    public void Play()
    {
        this.data.KoreographyPlayer.Play();
    }

    public void StartTrack(TrackType track, bool start)
    {
        var volumeParameter = track + "_Volume";
        this.data.Mixer.SetFloat(volumeParameter, start ? 0f : -80f);
    }

    public void FocusTrack(TrackType track)
    {
        this.currentFocusedTrack = track;
    }

    public void EndSection()
    {
        Debug.Log("[Song] Ending section=" + this.currentSection);

        this.canMoveToNextSection = true;
    }

    public bool HasPickup(Tile tile, bool includeAllLayers = false)
    {
        if (tile == null)
        {
            return false;
        }

        var pickup = this.currentPickups.Find(p => p.CurrentTile == tile);

        if (pickup == null)
        {
            return false;
        }

        if (!includeAllLayers && pickup.CurrentLayer != PickupLayer.Foreground)
        {
            return false;
        }

        return true;
    }

    private void ListenToEvents()
    {
        this.data.Koreographer.RegisterForEvents(TrackType.Beat.ToString(), this.HandleBeatTrackEvent);

        this.data.Koreographer.RegisterForEvents(TrackType.Beat.ToString(), ev => this.HandleTrackEvent(ev, TrackType.Beat));
        this.data.Koreographer.RegisterForEvents(TrackType.Piano.ToString(), ev => this.HandleTrackEvent(ev, TrackType.Piano));
        this.data.Koreographer.RegisterForEvents(TrackType.HiHat.ToString(), ev => this.HandleTrackEvent(ev, TrackType.HiHat));
        this.data.Koreographer.RegisterForEvents(TrackType.Triangle.ToString(), ev => this.HandleTrackEvent(ev, TrackType.Triangle));
        this.data.Koreographer.RegisterForEvents(TrackType.Pipa.ToString(), ev => this.HandleTrackEvent(ev, TrackType.Pipa));
    }

    private void HandleBeatTrackEvent(KoreographyEvent ev)
    {
        var moveToNextMeasure = ev.HasIntPayload() && ev.GetIntValue() >= 0;

        if (moveToNextMeasure)
        {
            this.currentBeat = 0;

            this.currentMeasure++;

            this.OnMeasureChange(this.currentMeasure);
        }
        else
        {
            this.currentBeat++;
        }

        if (!this.canMoveToNextSection)
        {
            this.currentScoreBeat++;
            this.OnScoreBeat(this.currentScoreBeat);
        }

        var measureProgress = this.currentMeasure % this.currentFocusedTrack.GetLengthInMeasures();

        if (this.currentBeat == 0 && measureProgress == 0 && this.canMoveToNextSection)
        {
            this.currentSection++;
            this.StartSection();
        }
    }

    private void StartLevel()
    {
        var newLevel = this.GetCurrentTrackLevel() + 1;
        var newLevelPatterns = this.data.Song.GetPickupPattern(this.currentFocusedTrack, newLevel);

        if (newLevelPatterns == null)
        {
            this.EndSection();
        }
        else
        {
            Debug.Log("[Song] Starting level=" + newLevel + " of track=" + this.currentFocusedTrack);

            this.SetCurrentTrackLevel(newLevel);

            foreach (var pattern in newLevelPatterns.Patterns)
            {
                var pickup = this.currentPickups.Find(p => p.CurrentLayer == PickupLayer.Background && p.CurrentTile.Coordinates == pattern.Coordinates);
                if (pickup == null)
                {
                    this.SpawnPickup(pattern, PickupLayer.Foreground);
                }
                else
                {
                    pickup.SetLayer(PickupLayer.Foreground);
                }
            }

            var nextLevelPatterns = this.data.Song.GetPickupPattern(this.currentFocusedTrack, newLevel + 1);

            if (nextLevelPatterns != null)
            {
                foreach (var pattern in nextLevelPatterns.Patterns)
                {
                    this.SpawnPickup(pattern, PickupLayer.Background);
                }
            }

            if (this.CanMoveToNextLevel())
            {
                this.StartLevel();
            }
        }
    }

    private void SpawnPickup(PickupPattern pattern, PickupLayer layer)
    {                
        var pickup = MusicManager.Instance.CreatePickup(pattern.Type);
        this.currentPickups.Add(pickup);

        var tile = Board.Instance.GetTile(pattern.Coordinates);
        pickup.Initialise(tile, layer, this.HandlePickupDestroy);
    }

    private void StartSection()
    {
        this.canMoveToNextSection = false;

        var section = this.data.Song.GetSection(this.currentSection);

        if (section == null)
        {
            this.End();
        }
        else    
        {
            Debug.Log("[Song] Starting section=" + this.currentSection);

            section.Start(this);
            this.StartLevel();
        }
    }

    private void End()
    {
        Debug.Log("[Song] Ending song");

        this.FocusTrack(TrackType.None);

        this.OnSongEnd();
    }

    private int GetCurrentMinNoteValue()
    {
        var level = this.GetCurrentTrackLevel();
        var levelPatterns = this.data.Song.GetPickupPattern(this.currentFocusedTrack, level);
        return levelPatterns == null ? -1 : levelPatterns.MinNoteValue;
    }

    private int GetCurrentTrackLevel()
    {
        int level;

        if (this.currentTrackLevels.TryGetValue(this.currentFocusedTrack, out level))
        {
            return level;
        }

        return -1;
    }

    private bool CanMoveToNextLevel()
    {
        return this.currentPickups.Find(p => p.CurrentLayer == PickupLayer.Foreground) == null;
    }

    private void SetCurrentTrackLevel(int level)
    {
        this.currentTrackLevels[this.currentFocusedTrack] = level;
    }

    private void HandleTrackEvent(KoreographyEvent ev, TrackType track)
    {
        if (track != this.currentFocusedTrack)
        {
            return;
        }

        var value = ev.HasIntPayload() ? ev.GetIntValue() : -1;

        if (value > this.GetCurrentMinNoteValue())
        {
            return;
        }

        OnTrackNoteEvent(track, value);
    }

    private void HandlePickupDestroy(Pickup pickup)
    {
        this.currentPickups.Remove(pickup);

        if (this.CanMoveToNextLevel())
        {
            this.StartLevel();
        }

        this.OnPickupDestroy(pickup);
    }
}
