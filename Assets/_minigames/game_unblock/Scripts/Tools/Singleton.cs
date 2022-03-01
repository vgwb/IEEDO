using UnityEngine;
namespace minigame.unblock
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T instance;

        public static bool IsInstance { get { return instance != null; } }

        protected virtual void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            instance = this as T;
            //        DontDestroyOnLoad(this);
        }

        public static T Instance
        {
            get
            {
                if (instance == null || instance.gameObject == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));
                    if (instance == null)
                    {
                        Debug.LogWarning("An instance of " + typeof(T) + " is needed in the scene, but there is none.");
                    }
                }
                return instance;
            }
        }
    }
}
