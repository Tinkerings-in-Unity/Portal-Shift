// using Code.Utility.Events;
using UnityEngine;

namespace Code.Utility
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        private static T instance;

        private static bool m_applicationIsQuitting = false;

        public static T GetInstance()
        {
            if (m_applicationIsQuitting) { return null; }

            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }

        public static bool TryGetInstance(out T instance)
        {
            instance = GetInstance();
            return instance != null;
        }

        /* IMPORTANT!!! To use Awake in a derived class you need to do it this way
         * protected override void Awake()
         * {
         *     base.Awake();
         *     //Your code goes here
         * }
         * */

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                SetDontDestroyOnLoad();
            }
            else if (instance != this as T)
            {
                Debug.LogError($"Duplicate MonoSingleton of {typeof(T)} type");
                Debug.Log(this);
             //   Destroy(this);
            }
            else
            {
                SetDontDestroyOnLoad();
            }

            if (transform.parent != null)
            {
                Debug.LogWarningFormat("Singleton of type {0} cannot be set DontDestroyOnLoad as it is a child of another object", instance.GetType().ToString());
            }
        }

        private void SetDontDestroyOnLoad()
        {
            instance.transform.SetParent(null, true);
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            instance = null;
            // m_applicationIsQuitting = true;
            
        }        
    }
}