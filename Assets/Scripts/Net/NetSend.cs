using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Net.Protos.Proto;
using UnityEngine;
using System.Threading;
using System.IO;

namespace Net
{
    class NetSend : NetBase
    {
        private List<RawPacket> mListSendBuff = new List<RawPacket>();

        public bool mConnectInitParam { get; private set; }
        private bool mAddToFirstPacket = false;

        private Thread mThread;

        public NetSend(NetSocket netSocket)
            : base(netSocket)
        {
        }

        public override void Init()
        {
            mConnectInitParam = false;
            mAddToFirstPacket = false;

            mListSendBuff.Clear();
        }

        public override void Close()
        {
            base.Close();

            if (mThread == null)
                return;

            //mThread.Abort();
            mThread = null;

            Init();

            ClearLastSendRawPacket(mTimeoutRawPacketList);

            if (mNetSocketParam.GetLoginStatus())
            {
                mTimeoutRawPacketList = mLastSendRawPacketList;
                mLastSendRawPacketList = new List<RawPacket>();
            }
            else
            {
                ClearLastSendRawPacket(mLastSendRawPacketList);
            }
        }

        public void SetConnectInitParam()
        {
            bool loginOK = mNetSocketParam.GetLoginStatus();

            if (loginOK)
            {
                mAddToFirstPacket = true;
                mNetSocketParam.LoginIn();
            }

            mConnectInitParam = true;

            if (mThread == null)
            {
                mThread = new Thread(ThreadRunSendData);
                mThread.Start();
            }
        }

        void ThreadRunSendData()
        {
            while (mConnectInitParam)
            {
                ThreadSendData();
                Thread.Sleep(1);
            }
        }

        private void ClearLastSendRawPacket(List<RawPacket> list)
        {
            if (list != null && list.Count > 0)
            {
                for (int i = 0, count = list.Count; i < count; i++)
                {
                    RawPacket packet = list[i];
                    packet.Release();
                }

                list.Clear();
            }
        }

        private List<RawPacket> mThreadSendList = new List<RawPacket>();
        private List<RawPacket> mTimeoutRawPacketList = new List<RawPacket>();
        private List<RawPacket> mLastSendRawPacketList = new List<RawPacket>();
        private int mLastSendIndex = 0;
        void ThreadSendData()
        {
            NetworkStream stream = InternalNetworkStream();
            if (stream != null && stream.CanWrite && mListSendBuff.Count > 0)
            {
                lock (mListSendBuff)
                {
                    mThreadSendList.Clear();
                    mThreadSendList.AddRange(mListSendBuff);
                    mListSendBuff.Clear();
                }

                try
                {
                    int newReceiveIndex = mNetSocket.mNetReceive.mReceiveIndex;
                    if (mLastSendIndex != newReceiveIndex)
                    {
                        ClearLastSendRawPacket(mLastSendRawPacketList);

                        mLastSendIndex = newReceiveIndex;
                    }

                    for (int i = 0, count = mThreadSendList.Count; i < count; i++)
                    {
                        RawPacket packet = mThreadSendList[i];
                        byte[] buff = packet.PackProtobuf(mProtoUtil);

                        int cmdID = packet.GetMsgID();

                        //Debug.LogError("cmdID:" + cmdID);

                        mNetSocketParam.MsgSendComplete(cmdID);
                        int askUniqueID = packet.mAskUniqueID;
                        mProtoUtil.ApplyUniqueID(askUniqueID);

                        if (packet.IsHeartPacket())
                        {
                            packet.Release();
                        }
                        else
                            mLastSendRawPacketList.Add(packet);

                        stream.Write(buff, 0, buff.Length);
                    }
                }
                catch (IOException exception)
                {
                    mSendError = true;
                    InternalSetConnectStatus(NetStatus.reconnect, "ThreadSendData ID:" + mNetSocketParam.mID + " msg:" + exception.Message, true);
                }
                catch (Exception exception2)
                {
                    Debug.LogError("ThreadSendData:" + exception2.StackTrace);
                }
            }
        }

        private bool mSendError;
        public override void Update()
        {
            if (mSendError)
            {
                mSendError = false;
                mNetSocketParam.ShowSendErrorTip();
            }
        }

        public void AddSendBuff(RawPacket packet)
        {
            lock (mListSendBuff)
            {
                if (mAddToFirstPacket)
                {
                    mAddToFirstPacket = false;

                    mListSendBuff.Insert(mListSendBuff.Count, packet);
                    if (mTimeoutRawPacketList.Count > 0)
                    {
                        mListSendBuff.InsertRange(mListSendBuff.Count, mTimeoutRawPacketList);
                        mTimeoutRawPacketList.Clear();
                    }
                }
                else
                {
                    mListSendBuff.Add(packet);
                }
            }
        }
    }
}
