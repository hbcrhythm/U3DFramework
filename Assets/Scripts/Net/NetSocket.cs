using UnityEngine;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Game.Utils;
using Net.Protos.Proto;
using Game.Utils.Timers;

namespace Net
{
    public enum NetStatus
    {
        disconnect,
        success,
        linking,
        reconnect,
        sendTimeout
    }

    class NetBase
    {
        protected bool mIsEditor = Application.isEditor;
        protected NetSocket mNetSocket;
        protected proto_util mProtoUtil;
        protected MsgManager mNetMsgManager;
        protected NetSocketParam mNetSocketParam;

        public NetBase()
        {

        }

        public NetBase(NetSocket netSocket)
        {
            this.mNetSocket = netSocket;
            this.mProtoUtil = mNetSocket.mProtoUtil;
            this.mNetMsgManager = netSocket.mNetMsgManager;
            this.mNetSocketParam = netSocket.mNetSocketParam;
        }

        public virtual void Init()
        {

        }

        public virtual void Update()
        {

        }

        protected bool InternalConnected()
        {
            return mNetSocket.Connected();
        }

        protected TcpClient InternalTcpClient()
        {
            return mNetSocket.GetTcpClient();
        }

        protected Socket InternalSocket()
        {
            return mNetSocket.GetSocket();
        }

        protected NetworkStream InternalNetworkStream()
        {
            return mNetSocket.GetNetworkStream();
        }

        protected void InternalSetConnectStatus(NetStatus status, string msg = "", bool showError = false)
        {
            mNetSocket.SetConnectStatus(status, msg, showError);
        }

        public virtual void Close()
        {

        }
    }

    class NetSocket
    {
        public NetStatus netState { get { return mNetConnect.mNetStatus; } }

        private NetConnect mNetConnect;
        public NetSend mNetSend { get; private set; }
        public NetReceive mNetReceive { get; private set; }

        public proto_util mProtoUtil;
        public MsgManager mNetMsgManager { get; private set; }
        public NetSocketParam mNetSocketParam { get; private set; }

        public NetSocket(NetSocketParam param)
        {
            mNetSocketParam = param;
            mProtoUtil = new proto_util();
            mNetMsgManager = new MsgManager();

            mNetConnect = new NetConnect(this);
            mNetSend = new NetSend(this);
            mNetReceive = new NetReceive(this);

            Init();
        }

        private void Init()
        {
            mNetConnect.Init();
            mNetSend.Init();
            mNetReceive.Init();
        }

        public void SetConnectInitParam(int baseValue, int addValue)
        {
            mProtoUtil.SetInitParam(baseValue, addValue);

            //mNetSend.SetConnectInitParam();
        }

        public void ConnectSuccessInit()
        {
            mNetReceive.ConnectSuccessInit();
        }

        public void InitIPAndPort(string ip, int port)
        {
            mNetConnect.InitIPAndPort(ip, port);
        }

        public void Send(RawPacket packet)
        {
            mNetSend.AddSendBuff(packet);
        }

        public void Update()
        {
            mNetSend.Update();
            mNetReceive.Update();

            mNetConnect.Update();
        }

        public bool Connected()
        {
            return mNetConnect.Connected();
        }

        public TcpClient GetTcpClient()
        {
            return mNetConnect.mTcpClient;
        }

        public Socket GetSocket()
        {
            return mNetConnect.mSocket;
        }

        public NetworkStream GetNetworkStream()
        {
            return mNetConnect.mNetworkStream;
        }

        public void SetConnectStatus(NetStatus status, string msg = "", bool showError = false)
        {
            mNetConnect.SetConnectStatus(status, msg, showError);
        }

        public void ReconnectManual()
        {
            mNetConnect.ReconnectManual();
        }

        public void Close()
        {
            Debug.Log("---NetSocket Close");
            mNetConnect.Close();
            Init();
        }
    }
}
