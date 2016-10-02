using UnityEngine;
using SonicBloom.Koreo;

public class MusicManager : BehaviourSingleton<MusicManager>
{
    private const string EventID_Bass = "Bass";
    private const string EventID_Drums = "Drums";
    private const string EventID_EGuitar = "EGuitar";
    private const string EventID_Piano = "Piano";

    public event KoreographyEventCallback OnBassEvent = delegate { };
    public event KoreographyEventCallback OnDrumsEvent = delegate { };
    public event KoreographyEventCallback OnEGuitarEvent = delegate { };
    public event KoreographyEventCallback OnPianoEvent = delegate { };

    [SerializeField]
    private Koreographer koreographer;

    private void Awake()
    {
        this.koreographer.RegisterForEvents(EventID_Bass, ev => this.OnBassEvent(ev));
        this.koreographer.RegisterForEvents(EventID_Drums, ev => this.OnDrumsEvent(ev));
        this.koreographer.RegisterForEvents(EventID_EGuitar, ev => this.OnEGuitarEvent(ev));
        this.koreographer.RegisterForEvents(EventID_Piano, ev => this.OnPianoEvent(ev));
    }
}
