using UnityEngine;

namespace Assets.Scripts.Infrastructure
{
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
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
                        _instance = Object.FindObjectOfType<T>();

                    return _instance;
                }
            }
            internal set { _instance = value; }
        }
    }
}