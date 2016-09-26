using UnityEngine;

public static class GameObjectUtility
{
    #region Instantiate


    public static GameObject InstantiateGameObject(string name, Transform parent = null)
    {
        var go = new GameObject(name);

        if (parent)
        {
            go.transform.parent = parent;
            ResetLocalTransform(go.transform, true);
        }

        return go;
    }

    public static T InstantiateComponent<T>(Transform parent = null) where T : Component
    {
        var go = InstantiateGameObject(typeof(T).Name, parent);

        return go.AddComponent<T>();
    }

    public static T InstantiatePrefab<T>(T prefab, Transform parent = null, bool resetTransformAfterParenting = true) where T : Component
    {
        Debug.Assert(prefab != null, prefab);

        var t = Object.Instantiate(prefab);

        Debug.Assert(t != null, prefab);

        if (parent)
        {
            if (resetTransformAfterParenting)
            {
                t.transform.parent = parent;
                ResetLocalTransform(t.transform, true);
            }
            else
            {
                t.transform.SetParent(parent, true);
            }
        }
        
        return t;
    }


    #endregion


    #region Transform


    public static void ResetLocalTransform(Transform t, bool includeScale)
    {
        Debug.Assert(t, t);

        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        
        if (includeScale)
        {
            t.localScale = Vector3.one;
        }
    }


    #endregion
}
