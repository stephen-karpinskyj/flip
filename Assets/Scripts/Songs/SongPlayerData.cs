using System;
using UnityEngine;
using UnityEngine.Audio;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;

[Serializable]
public class SongPlayerData
{
    [SerializeField]
    private Koreographer koreographer;

    [SerializeField]
    private MultiMusicPlayer koreographyPlayer;

    [SerializeField]
    private AudioMixer mixer;

    [SerializeField]
    private Song song;

    public Koreographer Koreographer
    {
        get { return this.koreographer; }
    }

    public MultiMusicPlayer KoreographyPlayer
    {
        get { return this.koreographyPlayer; }
    }

    public AudioMixer Mixer
    {
        get { return this.mixer; }
    }

    public Song Song
    {
        get { return this.song; }
    }
}
