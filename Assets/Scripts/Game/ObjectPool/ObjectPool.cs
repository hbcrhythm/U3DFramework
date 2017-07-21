using System;
using System.Collections.Generic;

namespace Game.ObjectPool
{
    class ObjectPool<T>
    {
        private LinkedList<T> mObjList;

        public ObjectPool()
        {

        }

        private void CheckInitList()
        {
            if (mObjList == null)
            {
                mObjList = new LinkedList<T>();
            }
        }

        public T Get(bool isCreate = true)
        {
            T tObject = default(T);
            if (mObjList == null || mObjList.Count == 0)
            {
                if (isCreate)
                    tObject = Activator.CreateInstance<T>();
            }
            else
            {
                tObject = mObjList.First.Value;
                mObjList.RemoveFirst();
            }

            return tObject;
        }

        public void Put(T tObj)
        {
            if (tObj != null)
            {
                CheckInitList();

                mObjList.AddFirst(tObj);
            }
        }

        public void ClearPool()
        {
            if (mObjList != null)
            {
                mObjList.Clear();
            }
        }

    }
}
