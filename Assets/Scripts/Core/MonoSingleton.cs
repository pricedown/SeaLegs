using UnityEditorInternal;
using UnityEngine;

namespace SeaLegs.Core
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance) return _instance;
                _instance = FindFirstObjectByType<T>();
                if (_instance) return _instance;
                _instance = new GameObject($"{nameof(T)} (Singleton)").AddComponent<T>();
                DontDestroyOnLoad(_instance.gameObject);
                return _instance;
            }
        }

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogWarning($"{nameof(T)} singleton already created, destroying this");
                Destroy(this.gameObject);
                return;
            }

            _instance = this as T;
            DontDestroyOnLoad(this.gameObject);

            Init();
        }

        protected abstract void Init();
    }
}