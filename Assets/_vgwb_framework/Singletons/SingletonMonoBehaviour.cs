using UnityEngine;

namespace vgwb.framework
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : Component
    {
        public static T I { get; private set; }

        public string TypeName { get; private set; }

        protected virtual void Awake()
        {
            TypeName = typeof(T).FullName;

            if (I != null)
            {
                if (I != this)
                {
                    Destroy(gameObject);
                }
                return;
            }

            I = this as T;

            Init();
        }

        void OnDestroy()
        {
            if (I == this)
            {
                Finalise();
            }
        }

        protected virtual void Init()
        {
        }

        protected virtual void Finalise()
        {
        }
    }
}
