using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuaInterface;

namespace Net.Lua
{
    public class LuaNetSocketParam
    {
        public LuaFunction loginIn;
        public LuaFunction reconnectTip;
        public LuaFunction connectTip;
        public LuaFunction receiveCallback;
        public List<PacketBytes> buffList = new List<PacketBytes>();
        public bool loginStatus;
    }
}
