using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using Net.Protos.Proto;
using System.IO;
using LuaInterface;

namespace Net
{
    public class PacketBytes
    {
        public int cmdID;
        public LuaByteBuffer byteBuffer;
    }

    class NetReceive : NetBase
    {
        private ushort mPackageLen = 0;
        private ushort mCMDID = 0;
        private int mReadPackageLen = 0;
        private byte[] mHeadBuff;
        private byte[] mBodyBuff;
        private Thread mThread;
        private List<PacketBytes> mPreReadObjList;
        private List<PacketBytes> buffList;
        private LuaFunction receiveCallback;
        private bool mThreadRunning;

        public string GetStatusDesc()
        {
            return "";
        }

        public NetReceive(NetSocket netSocket)
            : base(netSocket)
        {
            mHeadBuff = new byte[NetConstant.cPackageCMDLenSize];
            mPreReadObjList = new List<PacketBytes>();
        }

        public override void Init()
        {

        }

        public override void Close()
        {
            base.Close();

            if (mThread != null)
            {
                //mThread.Abort();
                mThreadRunning = false;
                mThread = null;
            }
        }

        public void ConnectSuccessInit()
        {
            mPreReadObjList.Clear();
            mPackageLen = 0;
            mCMDID = 0;
            mReadPackageLen = 0;

            if (mThread == null)
            {
                mThreadRunning = true;
                mThread = new Thread(ThreadRunReceiveData);
                mThread.Start();
            }

        }

        void ThreadRunReceiveData()
        {
            while (mThreadRunning)
            {
                ThreadReceiveData();
                Thread.Sleep(1);
            }
        }

        void ThreadReceiveData()
        {
            NetworkStream stream = InternalNetworkStream();
            if (stream != null && stream.CanRead && stream.DataAvailable)
            {
                try
                {
                    if (mPackageLen == 0)
                    {
                        int readSize = stream.Read(mHeadBuff, mReadPackageLen, NetConstant.cPackageCMDLenSize - mReadPackageLen);
                        if (readSize <= 0)
                            return;

                        mReadPackageLen += readSize;
                        if (mReadPackageLen >= NetConstant.cPackageCMDLenSize)
                        {
                            mProtoUtil.GetHeadData(mHeadBuff, out mCMDID, out mPackageLen);
                            mBodyBuff = new byte[mPackageLen];

                            if (mPackageLen == 0)
                            {
                                Unpack(mBodyBuff, mCMDID);
                                mBodyBuff = null;
                                mReadPackageLen = 0;
                            }
                            else
                            {
                                mReadPackageLen = 0;
                            }
                        }

                    }
                    else
                    {
                        int readSize = stream.Read(mBodyBuff, mReadPackageLen, mPackageLen - mReadPackageLen);
                        if (readSize <= 0)
                            return;

                        mReadPackageLen += readSize;
                        if (mReadPackageLen >= mPackageLen)
                        {
                            Unpack(mBodyBuff, mCMDID);
                            mBodyBuff = null;
                            mReadPackageLen = 0;
                            mPackageLen = 0;
                        }
                    }
                }
                catch (IOException exception)
                {
                    InternalSetConnectStatus(NetStatus.reconnect, "ThreadReceiveData ID:" + mNetSocketParam.mID + " msg:" + exception.Message, true);
                }
                catch (Exception exception2)
                {
                    Debug.LogError("ThreadReceiveData:" + exception2.StackTrace);
                }
            }
        }

        void ReceiveData()
        {
            if (buffList == null)
            {
                buffList = mNetSocketParam.mLuaNetSocketParam.buffList;
                receiveCallback = mNetSocketParam.mLuaNetSocketParam.receiveCallback;
            }

            if (mPreReadObjList.Count > 0)
            {
                lock (mPreReadObjList)
                {
                    buffList.Clear();
                    buffList.AddRange(mPreReadObjList);

                    mPreReadObjList.Clear();
                }

                //调用lua
                receiveCallback.Call();
            }

        }

        public override void Update()
        {
            if (InternalConnected())
            {
                if (mIsEditor)
                {
                    ReceiveData();
                }
                else
                {
                    try
                    {
                        ReceiveData();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Receive err:" + ex.StackTrace);
                    }
                }
            }
        }

        public int mReceiveIndex { get; private set; }
        void Unpack(byte[] buff, ushort cmdID)
        {
            mReceiveIndex++;
            PacketBytes obj = new PacketBytes();
            obj.cmdID = cmdID;
            obj.byteBuffer = new LuaByteBuffer(buff);

            lock (mPreReadObjList)
            {
                mPreReadObjList.Add(obj);
            }
        }
    }
}
