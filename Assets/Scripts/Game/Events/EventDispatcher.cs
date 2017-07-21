using System;
using System.Collections.Generic;
using Game.Core;
using UnityEngine;

namespace Assets.Scripts.Game.Events
{
    public class EventDispatcher : Singleton<EventDispatcher>
    {
        public delegate void EventCallback();
        public delegate void EventCallback<T>(T arg1);
        public delegate void EventCallback<T1, T2>(T1 arg1, T2 arg2);
        public delegate void EventCallback<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);

        //private Dictionary<EventConstant, List<Delegate>> mEventListeners = new Dictionary<EventConstant, List<Delegate>>();
        private List<System.Delegate>[] mCallBacks = new List<System.Delegate>[(int)EventConstant.MAX_COUNT];

        public void AddEventListener(EventConstant typeCode, EventCallback listener)
        {
            RegisterEventListener(typeCode, listener);
        }

        public void AddEventListener<T1>(EventConstant typeCode, EventCallback<T1> listener)
        {
            RegisterEventListener(typeCode, listener);
        }

        public void AddEventListener<T1, T2>(EventConstant typeCode, EventCallback<T1, T2> listener)
        {
            RegisterEventListener(typeCode, listener);
        }

        public void AddEventListener<T1, T2, T3>(EventConstant typeCode, EventCallback<T1, T2, T3> listener)
        {
            RegisterEventListener(typeCode, listener);
        }

        public void RegisterEventListener(EventConstant typeCode, Delegate listener)
        {
            int id = (int)typeCode;
            List<System.Delegate> listeners = mCallBacks[id];

            if (listeners == null)
            {
                listeners = new List<Delegate>();
                mCallBacks[id] = listeners;
            }

            if (listeners.Contains(listener) == false)
            {
                listeners.Add(listener);
            }
        }

        public void RemoveEventListener(EventConstant typeCode, EventCallback listener)
        {
            DeleteEventListener(typeCode, listener);
        }

        public void RemoveEventListener<T1>(EventConstant typeCode, EventCallback<T1> listener)
        {
            DeleteEventListener(typeCode, listener);
        }

        public void RemoveEventListener<T1, T2>(EventConstant typeCode, EventCallback<T1, T2> listener)
        {
            DeleteEventListener(typeCode, listener);
        }

        public void RemoveEventListener<T1, T2, T3>(EventConstant typeCode, EventCallback<T1, T2, T3> listener)
        {
            DeleteEventListener(typeCode, listener);
        }

        public void DeleteEventListener(EventConstant typeCode, Delegate listener)
        {
            int id = (int)typeCode;
            List<System.Delegate> listeners = mCallBacks[id];

            if (listeners != null)
            {
                listeners.Remove(listener);
            }
        }

        public void Dispatch(EventConstant typeCode)
        {
            Delegate[] invocationList = GetListeners(typeCode);

            DebugEvent(typeCode, invocationList);

            if (invocationList != null)
            {
                for (int i = 0, count = invocationList.Length; i < count; ++i)
                {
                    ((EventCallback)invocationList[i])();
                }
            }
        }

        public void Dispatch<T1>(EventConstant typeCode, T1 t1)
        {
            Delegate[] invocationList = GetListeners(typeCode);

            DebugEvent(typeCode, invocationList);

            if (invocationList != null)
            {
                for (int i = 0, count = invocationList.Length; i < count; ++i)
                {
                    ((EventCallback<T1>)invocationList[i])(t1);
                }
            }
        }

        public void Dispatch<T1, T2>(EventConstant typeCode, T1 t1, T2 t2)
        {
            Delegate[] invocationList = GetListeners(typeCode);

            DebugEvent(typeCode, invocationList);

            if (invocationList != null)
            {
                for (int i = 0, count = invocationList.Length; i < count; ++i)
                {
                    ((EventCallback<T1, T2>)invocationList[i])(t1, t2);
                }
            }
        }

        public void Dispatch<T1, T2, T3>(EventConstant typeCode, T1 t1, T2 t2, T3 t3)
        {
            Delegate[] invocationList = GetListeners(typeCode);

            DebugEvent(typeCode, invocationList);

            if (invocationList != null)
            {

                for (int i = 0, count = invocationList.Length; i < count; ++i)
                {
                    ((EventCallback<T1, T2, T3>)invocationList[i])(t1, t2, t3);
                }
            }
        }

        private Delegate[] GetListeners(EventConstant typeCode)
        {
            int id = (int)typeCode;
            List<System.Delegate> listeners = mCallBacks[id];

            if (listeners != null)
            {
                return listeners.ToArray();
            }

            return null;
        }

        private void DebugEvent(EventConstant typeCode, Delegate[] invocationList)
        {
        }
    }
}
