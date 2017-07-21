using Game.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Utils.Timers
{
    class GameTimerManager : Singleton<GameTimerManager>
    {
        LinkedList<GameTimer> mRealTimeList = new LinkedList<GameTimer>();
        LinkedList<GameTimer> mScaleTimeList = new LinkedList<GameTimer>();

        HashSet<GameTimer> mChangeList = new HashSet<GameTimer>();

        public double mRealTimeSinceStartup { get; private set; }
        public double mScaleTimeSinceStartup { get; private set; }

        public GameTimerManager()
        {
            mRealTimeSinceStartup = new TimeSpan(DateTime.Now.Ticks).TotalSeconds;
        }

        public void SetGamePause(bool value)
        {
            if (value)
            {
                SetTimeScale(0f);
            }
            else
            {
                SetTimeScale(1.0f);
            }
        }

        public void SetTimeScale(float value)
        {
            Time.timeScale = value;
        }

        public void ClearOnChangeScene()
        {
            ExecuteChangeTimerList();
            InternalClearOnChangeScene(mRealTimeList);
            InternalClearOnChangeScene(mScaleTimeList);
        }

        private void InternalClearOnChangeScene(LinkedList<GameTimer> timerList)
        {
            LinkedListNode<GameTimer> node = timerList.First;

            while (node != null)
            {
                GameTimer timer = node.Value;

                node = node.Next;

                if (timer.mStopOnChangeScene)
                {
                    timer.Dispose();
                }
            }
        }

        internal void AddGameTimer(GameTimer timer)
        {
            AddChangeTimer(timer);
        }

        private void InternalAddGameTimer(GameTimer timer)
        {
            if (timer.mIgnoreOnPause)
            {
                if (timer.mDelay > 0)
                {
                    timer.mNextExecuteTime = mScaleTimeSinceStartup + timer.mDelay;
                }

                InternalAddGameTimerList(mScaleTimeList, timer);
            }
            else
            {
                if (timer.mDelay > 0)
                {
                    timer.mNextExecuteTime = mRealTimeSinceStartup + timer.mDelay;
                }

                InternalAddGameTimerList(mRealTimeList, timer);
            }
        }

        private void InternalAddGameTimerList(LinkedList<GameTimer> timerList, GameTimer timer)
        {
            InternalRemoveGameTimer(timer);

            for (LinkedListNode<GameTimer> node = timerList.First; node != null; node = node.Next)
            {
                GameTimer nodeValue = node.Value;

                if (timer.mNextExecuteTime < nodeValue.mNextExecuteTime)
                {
                    timer.mSelfList = timerList;
                    timer.mSelfNode = timerList.AddBefore(node, timer);
                    return;
                }
            }

            timer.mSelfList = timerList;
            timer.mSelfNode = timerList.AddLast(timer);
        }

        internal void RemoveGameTimer(GameTimer timer)
        {
            AddChangeTimer(timer);
        }

        private void InternalRemoveGameTimer(GameTimer timer)
        {
            if (timer.mSelfNode != null)
                timer.mSelfList.Remove(timer.mSelfNode);

            timer.mSelfList = null;
            timer.mSelfNode = null;
        }

        private void AddChangeTimer(GameTimer timer)
        {
            lock (mChangeList)
            {
                mChangeList.Add(timer);
            }
        }

        private void InternalExecute(LinkedList<GameTimer> timerList, double curTime)
        {
            LinkedListNode<GameTimer> node = timerList.First;

            while (node != null)
            {
                GameTimer timer = node.Value;
                if (timer.mNextExecuteTime <= curTime)
                {
                    node = node.Next;
                    AddChangeTimer(timer);

                    if (timer.mRunning)
                        timer.Execute();

                    continue;
                }
                else
                    break;
            }
        }

        public void Execute()
        {
            lock (mChangeList)
            {
                mScaleTimeSinceStartup += Time.deltaTime;
                mRealTimeSinceStartup = new TimeSpan(DateTime.Now.Ticks).TotalSeconds;

                if (mRealTimeList.Count > 0)
                {
                    InternalExecute(mRealTimeList, mRealTimeSinceStartup);
                }

                if (mScaleTimeList.Count > 0)
                {
                    InternalExecute(mScaleTimeList, mScaleTimeSinceStartup);
                }

                ExecuteChangeTimerList();
            }
        }

        private void ExecuteChangeTimerList()
        {
            if (mChangeList.Count > 0)
            {
                foreach (GameTimer timer in mChangeList)
                {
                    if (timer.mRunning)
                    {
                        InternalAddGameTimer(timer);
                    }
                    else
                    {
                        InternalRemoveGameTimer(timer);
                    }
                }

                mChangeList.Clear();
            }
        }
    }
}
