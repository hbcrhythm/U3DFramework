using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Game.Events;
using UnityEngine;

namespace Assets.Scripts.Game.GameUpdate
{
    class LaunchViewBase : EventDispatcherInterface
    {
        protected GameObject mGameObject;
        protected Transform mTransform;

        public Transform Find(string path)
        {
            return mTransform.Find(path);
        }

        public T GetComponent<T>(string path)
        {
            return Find(path).GetComponent<T>();
        }
    }
}
