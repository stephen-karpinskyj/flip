using System;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

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

    private readonly SongPlayerData data;

    private int currentBeat;
    private int currentScoreBeat;
    private int currentMeasure;
    private int currentSection;
    private TrackType currentFocusedTrack;

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

    public TrackType CurrentFocusedTrack
    {
        get { return this.currentFocusedTrack; }
    }

    public bool HasStarted
    {
        get { return this.data.KoreographyPlayer.IsPlaying; }
    }

    public SongPlayer(SongPlayerData data)
    {
        this.data = data;

        this.currentBeat = -1;
        this.currentScoreBeat = 0;
        this.currentMeasure = -1;
        this.currentSection = -1;
        this.currentFocusedTrack = TrackType.None;

        this.currentTrackLevels = new Dictionary<TrackType, int>();
        this.canMoveToNextSection = true;

        this.ListenToEvents();

        this.SpawnNextSectionStartPatterns();

        Board.Instance.OnPickupDestroy += this.HandlePickupDestroy; // TODO: Unsub
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
        var newLevel = this.GetTrackLevel(this.currentFocusedTrack) + 1;
        var newLevelPatterns = this.data.Song.GetPickupPattern(this.currentFocusedTrack, newLevel);

        if (newLevelPatterns == null)
        {
            this.EndSection();

            this.SpawnNextSectionStartPatterns();
        }
        else
        {
            Debug.Log("[Song] Starting level=" + newLevel + " of track=" + this.currentFocusedTrack);

            this.SetCurrentTrackLevel(newLevel);

            foreach (var pattern in newLevelPatterns.Patterns)
            {
                Board.Instance.AddForegroundPickup(pattern);
            }

            this.SpawnNextLevelPatterns(this.currentFocusedTrack);

            if (this.CanMoveToNextLevel())
            {
                this.StartLevel();
            }
        }
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
        var level = this.GetTrackLevel(this.currentFocusedTrack);
        var levelPatterns = this.data.Song.GetPickupPattern(this.currentFocusedTrack, level);
        return levelPatterns == null ? -1 : levelPatterns.MinNoteValue;
    }

    private int GetTrackLevel(TrackType track)
    {
        int level;

        return this.currentTrackLevels.TryGetValue(track, out level) ? level : -1;
    }

    private bool CanMoveToNextLevel()
    {
        return !Board.Instance.HasPickup(false);
    }

    private void SetCurrentTrackLevel(int level)
    {
        this.currentTrackLevels[this.currentFocusedTrack] = level;
    }

    public void SpawnNextLevelPatterns(TrackType track)
    {
        var nextLevel = this.GetTrackLevel(track) + 1;
        var nextLevelPatterns = this.data.Song.GetPickupPattern(track, nextLevel);

        if (nextLevelPatterns != null)
        {
            foreach (var pattern in nextLevelPatterns.Patterns)
            {
                Board.Instance.AddBackgroundPickup(pattern);
            }
        }
    }

    private void SpawnNextSectionStartPatterns()
    {
        var nextSection = this.data.Song.GetSection(this.currentSection + 1);

        if (nextSection != null)
        {
            var track = nextSection.FindFocusedTrack();
            this.SpawnNextLevelPatterns(track);
        }
    }

    private void HandleTrackEvent(KoreographyEvent ev, TrackType track)
    {
        if (this.IsEndingSection || track != this.currentFocusedTrack)
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
        if (this.CanMoveToNextLevel())
        {
            this.StartLevel();
        }
    }
}
