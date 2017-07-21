using UnityEngine;
using System.IO;

namespace Game.ObjectPool
{
    class RecycleMS : MemoryStream, IPool
    {
        public static bool sThreadLock = false;

        public RecycleMS()
        {

        }

        public static RecycleMS CreateInstance()
        {
            if (sThreadLock)
            {
                lock (ObjectPoolManager.sMemoryStreamPool)
                {
                    return ObjectPoolManager.sMemoryStreamPool.Get();
                }
            }
            else
            {
                return ObjectPoolManager.sMemoryStreamPool.Get();
            }

        }

        public void Release()
        {
            this.SetLength(0);

            if (sThreadLock)
            {
                lock (ObjectPoolManager.sMemoryStreamPool)
                {
                    ObjectPoolManager.sMemoryStreamPool.Put(this);
                }
            }
            else
            {
                ObjectPoolManager.sMemoryStreamPool.Put(this);
            }

        }
    }
}
