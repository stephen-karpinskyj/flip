using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SongSection
{
    [SerializeField]
    private List<SongAction> onStart;

    public void Start(SongPlayer song)
    {
        foreach (var action in this.onStart)
        {
            action.Execute(song);
        }
    }
}
