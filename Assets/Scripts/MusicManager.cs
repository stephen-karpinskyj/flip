using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;

public class MusicManager : BehaviourSingleton<MusicManager>
{
    public const string EventID_Beat = "Beat";

    public const string EventID_Piano = "Piano";
    public const string EventID_HiHat = "HiHat";
    public const string EventID_Triangle = "Triangle";
    public const string EventID_Pipa = "Pipa";

    public delegate void BeatEventCallback(int beat, int measure);
    public event BeatEventCallback OnBeatEvent = delegate { };

    public delegate void TrackNoteEventCallback(string track, int value);
    public event TrackNoteEventCallback OnTrackNoteEvent = delegate { };

    public delegate void SectionStartCallback(int section);
    public event SectionStartCallback OnSectionStart = delegate { };

    [SerializeField]
    private Koreographer koreographer;

    [SerializeField]
    private MultiMusicPlayer player;

    [SerializeField]
    private AudioMixer mixer;

    [SerializeField]
    private Pickup pickupPrefab;

    private int numMeasuresPerSection = 8;
    private int numMeasuresToSpawnPickups = 8;
    private const int NumSections = 3;

    private int currentBeat;
    private int currentMeasure;
    private int currentSection;

    private bool isSpawningPickups;

    private Dictionary<Vector2I, Pickup> pickups = new Dictionary<Vector2I, Pickup>();
    private List<Tile> tempTiles = new List<Tile>();

    private bool[] isListeningTracks = new bool[4];

    private void Awake()
    {
        this.currentBeat = -1;
        this.currentMeasure = -1;
        this.currentSection = -1;

        this.koreographer.RegisterForEvents(EventID_Beat, this.HandleBeatTrackEvent);

        this.koreographer.RegisterForEvents(EventID_Piano, ev => this.HandleTrackEvent(ev, EventID_Piano));
        this.koreographer.RegisterForEvents(EventID_HiHat, ev => this.HandleTrackEvent(ev, EventID_HiHat));
        this.koreographer.RegisterForEvents(EventID_Triangle, ev => this.HandleTrackEvent(ev, EventID_Triangle));
        this.koreographer.RegisterForEvents(EventID_Pipa, ev => this.HandleTrackEvent(ev, EventID_Pipa));

        this.OnTrackNoteEvent += this.HandleTrackNoteEvent;
        this.OnSectionStart += this.HandleSectionStart;
    }

    private void Start()
    {
        this.player.Play();
    }

    private static int GetTrackIndex(string track)
    {
        switch (track)
        {
            case EventID_Piano: return 0;
            case EventID_HiHat: return 1;
            case EventID_Triangle: return 2;
            case EventID_Pipa: return 3;
        }

        return -1;
    }

    private void HandleBeatTrackEvent(KoreographyEvent ev)
    {
        this.currentBeat++;

        if (ev.HasIntPayload() && ev.GetIntValue() >= 0)
        {
            this.currentBeat = 0;

            this.currentMeasure++;
            this.currentMeasure %= numMeasuresPerSection;
        }

        if (this.currentBeat == 0 && this.currentMeasure >= this.numMeasuresToSpawnPickups)
        {
            this.isSpawningPickups = false;
        }

        if (this.currentBeat == 0 && this.currentMeasure == 0)
        {
            if (this.CanMoveToNextSection())
            {
                this.currentSection++;
                Debug.Log("[Music] Changing to section=" + this.currentSection);

                this.isSpawningPickups = true;
                this.OnSectionStart(this.currentSection);
            }
        }

        this.OnBeatEvent(this.currentBeat, this.currentMeasure);
    }

    private void StartTrack(string track, bool start)
    {
        var volumeParameter = track + "_Volume";
        this.mixer.SetFloat(volumeParameter, start ? 0f : -80f);
    }

    private void ListenToTrack(string track, bool focus)
    {
        var index = GetTrackIndex(track);
        this.isListeningTracks[index] = focus;
    }

    private void HandleTrackEvent(KoreographyEvent ev, string track)
    {
        var trackIndex = GetTrackIndex(track);

        if (this.isListeningTracks[trackIndex])
        {
            var value = ev.HasIntPayload() ? ev.GetIntValue() : -1;
            OnTrackNoteEvent(track, value);
        }
    }

    private void HandleTrackNoteEvent(string track, int value)
    {
        if (value == -1 || !this.isSpawningPickups)
        {
            return;
        }

        // Prepare spawn tile list
        {
            this.tempTiles.Clear();

            Board.Instance.ForEachTile(tile => this.tempTiles.Add(tile));

            this.tempTiles.Remove(Traveller.Instance.CurrentTile);
            const int ExcludeCount = 15;
            var i = 0;
            Traveller.Instance.CurrentPath.ForEachTile(tile =>
            {
                if (i >= ExcludeCount)
                {
                    return;
                }

                i++;
                this.tempTiles.Remove(tile);
            });

            foreach (var kv in this.pickups)
            {
                this.tempTiles.RemoveAll(tile => tile.Coordinates == kv.Key);
            }
        }

        if (this.tempTiles.Count <= 0)
        {
            Debug.LogWarning("[Music] Cannot spawn pickup for track=" + track + ", no available tiles");
            return;
        }

        var randomTile = this.tempTiles[Random.Range(0, this.tempTiles.Count)];
        var randomCoords = randomTile.Coordinates;
        Debug.Assert(!this.pickups.ContainsKey(randomCoords), this);

        var pickup = GameObjectUtility.InstantiatePrefab(this.pickupPrefab, this.transform);
        pickup.Initialise(randomTile, p => this.pickups.Remove(p.CurrentTile.Coordinates));
        this.pickups[randomCoords] = pickup;
    }

    private bool CanMoveToNextSection()
    {
        return this.currentSection < NumSections && this.pickups.Count <= 0;
    }

    private void HandleSectionStart(int section)
    {
        if (section == 0)
        {
            this.numMeasuresPerSection = 8;
            this.numMeasuresToSpawnPickups = 4;

            this.ListenToTrack(EventID_Piano, true);
            this.StartTrack(EventID_Piano, true);

            this.StartTrack(EventID_HiHat, false);
            this.StartTrack(EventID_Triangle, false);
            this.StartTrack(EventID_Pipa, false);
        }
        else if (section == 1)
        {
            this.numMeasuresPerSection = 8;
            this.numMeasuresToSpawnPickups = 4;

            this.ListenToTrack(EventID_Piano, false);

            this.StartTrack(EventID_HiHat, true);
            this.StartTrack(EventID_Triangle, true);
            //this.ListenToTrack(EventID_HiHat, true);
            this.ListenToTrack(EventID_Triangle, true);
        }
        else if (section == 2)
        {
            this.numMeasuresPerSection = 16;
            this.numMeasuresToSpawnPickups = 8;

            //this.ListenToTrack(EventID_HiHat, false);
            this.ListenToTrack(EventID_Triangle, false);

            this.StartTrack(EventID_Pipa, true);
            this.ListenToTrack(EventID_Pipa, true);
        }
        else if (section == NumSections)
        {
            this.ListenToTrack(EventID_Pipa, false);
            // TODO: End
        }
    }
}
