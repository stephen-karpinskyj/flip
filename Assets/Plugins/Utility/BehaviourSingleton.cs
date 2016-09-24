using UnityEngine;

public abstract class BehaviourSingleton<T> : BaseMonoBehaviour where T : MonoBehaviour
{
    protected static T instance;

    public static T Instance
    {
        get
        {
            if (!instance)
            {
                instance = GameObjectUtility.InstantiateComponent<T>();
                Debug.Assert(instance != null, instance);
                
                Object.DontDestroyOnLoad(instance);
            }

            return instance;
        }
    }

    public static bool Exists
    {
        get { return instance != null; }
    }
}
