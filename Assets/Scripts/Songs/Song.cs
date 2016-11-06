using System.Collections.Generic;
using UnityEngine;

public class Song : ScriptableObject
{
    [SerializeField]
    private List<PickupPatternGroup> pickupPatterns;

    [SerializeField]
    private SongSection[] songSections;

    public PickupPatternLevel GetPickupPattern(TrackType track, int level)
    {
        var group = this.pickupPatterns.Find(g => g.Track == track);

        return group == null ? null : group.GetPatterns(level);
    }

    public SongSection GetSection(int index)
    {
        if (index < 0 || index >= this.songSections.Length)
        {
            return null;
        }

        return this.songSections[index];
    }

    #if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/Song")]
    public static void CreateMyAsset()
    {
        var asset = ScriptableObject.CreateInstance<Song>();

        UnityEditor.AssetDatabase.CreateAsset(asset, "Assets/Songs/New Song.asset");
        UnityEditor.AssetDatabase.SaveAssets();

        UnityEditor.EditorUtility.FocusProjectWindow();
        UnityEditor.Selection.activeObject = asset;
    }
    #endif
}
