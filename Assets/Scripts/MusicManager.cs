using UnityEngine;
using SonicBloom.Koreo;

public class MusicManager : BehaviourSingleton<MusicManager>
{
    private const string EventID_Piano = "Piano";
    private const string EventID_HiHat = "HiHat";
    private const string EventID_Triangle = "Triangle";
    private const string EventID_Pipa = "Pipa";

    public event KoreographyEventCallback OnPianoEvent = delegate { };
    public event KoreographyEventCallback OnHiHatEvent = delegate { };
    public event KoreographyEventCallback OnTriangleEvent = delegate { };
    public event KoreographyEventCallback OnPipaEvent = delegate { };

    [SerializeField]
    private Koreographer koreographer;

    private void Awake()
    {
        this.koreographer.RegisterForEvents(EventID_Piano, ev => this.OnPianoEvent(ev));
        this.koreographer.RegisterForEvents(EventID_HiHat, ev => this.OnHiHatEvent(ev));
        this.koreographer.RegisterForEvents(EventID_Triangle, ev => this.OnTriangleEvent(ev));
        this.koreographer.RegisterForEvents(EventID_Pipa, ev => this.OnPipaEvent(ev));
    }
}
