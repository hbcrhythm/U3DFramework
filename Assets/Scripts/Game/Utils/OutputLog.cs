using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Game.Utils
{
    public sealed class OutputLog : MonoBehaviour
    {
        public static bool sRegister = false;
        private static string mOutPath;

        private List<string> mLogList;
        private List<string> mThreadLogList;

        private Thread mThread;

        void Awake()
        {
            sRegister = true;
            mOutPath = Application.persistentDataPath + "/outputLog.txt";

            if (Application.isEditor)
            {
                //Application.logMessageReceived += HandleLog;
            }
            else
            {
                //BuglyAgent.RegisterLogCallback(HandleLog);
            }

            if (System.IO.File.Exists(mOutPath))
            {
                File.Delete(mOutPath);
            }

            mLogList = new List<string>();
            mThreadLogList = new List<string>();

            mThread = new Thread(ThreadUpdate);
            mThread.Start();
        }

        void ThreadUpdate()
        {
            while (true)
            {
                if (mLogList.Count > 0)
                {
                    lock (mLogList)
                    {
                        mThreadLogList.Clear();
                        mThreadLogList.AddRange(mLogList);
                        mLogList.Clear();
                    }

                    try
                    {
                        using (StreamWriter writer = new StreamWriter(mOutPath, true, Encoding.UTF8))
                        {
                            for (int i = 0, count = mThreadLogList.Count; i < count; i++)
                            {
                                writer.WriteLine(mThreadLogList[i]);
                            }
                        }

                        mThreadLogList.Clear();
                    }
                    catch (System.Exception ex)
                    {

                    }
                }

                Thread.Sleep(33);
            }
        }

        void Start()
        {
            string str = DateTime.Now.ToString();
            Debug.Log(str + "...启动游戏..." + mOutPath);
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            lock (mLogList)
            {
                mLogList.Add(logString);
            }
        }
    }
}
