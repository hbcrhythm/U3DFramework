using Game.Utils;
using Net.Protos.Proto;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using Net.Lua;

namespace Net
{
    public static class NetInterface
    {
        private static NetSocket sNetSocket = new NetSocket(new NetSocketParam());

        public static void InitLuaSocketParam(LuaNetSocketParam luaSocketParam)
        {
            sNetSocket.mNetSocketParam.mLuaNetSocketParam = luaSocketParam;
        }

        public static void InitIPAndPort(string ip, int port)
        {
            sNetSocket.InitIPAndPort(ip, port);
        }

        public static void CloseConnect()
        {
            sNetSocket.Close();
        }

        public static void SetConnectInitParam(int baseValue, int addValue)
        {
            sNetSocket.SetConnectInitParam(baseValue, addValue);
        }

        public static void SendData(LuaRawPacket rawPacket)
        {
            sNetSocket.Send(new RawPacket(rawPacket));
        }

        public static void Update()
        {
            sNetSocket.Update();
        }

        public static bool Connected()
        {
            return sNetSocket.Connected();
        }

        public static void ReconnectManual()
        {
            sNetSocket.ReconnectManual();
        }

        public static void SendTimeout()
        {
            sNetSocket.SetConnectStatus(NetStatus.sendTimeout, "心跳包超时", true);
        }
    }
}
