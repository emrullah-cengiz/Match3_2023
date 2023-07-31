using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Infrastructure
{
    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        private static readonly object lock_obj = new();

        private static T _instance;

        public static T Instance
        {
            get
            {
                lock (lock_obj)
                {
                    if (_instance == null)
                    {
                        _instance = Resources.Load<T>("_Game/" + typeof(T).ToString());

                        if (_instance == null)
                            Debug.LogError($"ScriptableObject instance of {typeof(T)} not found on Resource/_Game folder!");
                    }

                    return _instance;
                }
            }
            internal set { _instance = value; }
        }
    }
}