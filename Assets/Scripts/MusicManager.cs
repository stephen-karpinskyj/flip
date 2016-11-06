using UnityEngine;
using System.Collections.Generic;

public class MusicManager : BehaviourSingleton<MusicManager>
{
    [SerializeField]
    private List<PickupPrefab> pickupPrefabs;

    public SongPlayer Player { get; private set; }

    public void AssignPlayer(SongPlayer player)
    {
        this.Player = player;
    }

    public Pickup CreatePickup(PickupType type)
    {
        return this.pickupPrefabs.Find(p => p.Type == type).Instantiate(this.transform);
    }
}
