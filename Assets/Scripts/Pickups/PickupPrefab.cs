using System;
using UnityEngine;

[Serializable]
public class PickupPrefab
{
    [SerializeField]
    private PickupType type;

    [SerializeField]
    private Pickup prefab;

    public PickupType Type
    {
        get { return this.type; }
    }

    public Pickup Instantiate(Transform transform)
    {        
        return GameObjectUtility.InstantiatePrefab(this.prefab, transform);
    }
}
