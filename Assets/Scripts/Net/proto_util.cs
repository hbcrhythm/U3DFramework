using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Net;
using System.Collections.Generic;

using Game.ObjectPool;
using Net.Protos.Proto;
using Game.Utils;

namespace Net
{
    public class proto_util
    {
        private int mMinUniqueId = 0;// 序列号起始值
        private int mUniqueAdd = 0;// 序列号递增幅度
        private int mMaxUniqueId = 0;// 最大序列号
        private int mCurrentUniqueId = 0;// 当前的序列号

        //请求序列ID，只有发送成功之后才会让序列ID自增
        public int AskUniqueID()
        {
            int tempID = mCurrentUniqueId + mUniqueAdd;
            if (tempID > mMaxUniqueId)
            {
                tempID = mMinUniqueId;
            }

            return tempID;
        }

        //发送成功后，序列ID要更新
        public void ApplyUniqueID(int id)
        {
            mCurrentUniqueId = id;
        }

        public void SetInitParam(int minUniqueID, int uniqueAdd)
        {
            //Debug.LogError(string.Format("SetInitParam minUniqueID:{0},uniqueAdd:{1},this:{2}", minUniqueID, uniqueAdd,this.GetHashCode()));

            mMinUniqueId = minUniqueID;
            mUniqueAdd = uniqueAdd;
            mMaxUniqueId = int.MaxValue;
            mCurrentUniqueId = mMinUniqueId;
        }

        private MemoryStream mHeadMs;
        public void GetHeadData(byte[] buff, out ushort cmd, out ushort packageLen)
        {
            if (mHeadMs == null)
            {
                mHeadMs = new MemoryStream(NetConstant.cPackageCMDLenSize);
            }
            mHeadMs.Position = 0;

            mHeadMs.Write(buff, 0, NetConstant.cPackageCMDLenSize);
            mHeadMs.Position = 0;

            packageLen = (ushort)(readUShort(mHeadMs) - NetConstant.cPackageCMDLenSize);
            cmd = readUShort(mHeadMs);
        }

        internal static void EncryptPackage(MemoryStream encryptMsg, byte[] unEncryptPackage)
        {
            int count = unEncryptPackage.Length;

            if (count > 0)
                encryptMsg.Write(unEncryptPackage, 0, count);
        }

        private static void readStream(MemoryStream oldStream, uint offset)
        {
            oldStream.Position = oldStream.Position + offset;
        }

        public static sbyte readByte(MemoryStream data)
        {
            sbyte tmpvalue;
            tmpvalue = (sbyte)data.GetBuffer()[data.Position];
            readStream(data, sizeof(sbyte));
            return tmpvalue;
        }

        public static byte readUByte(MemoryStream data)
        {
            byte tmpvalue;
            tmpvalue = (byte)data.GetBuffer()[data.Position];
            readStream(data, sizeof(sbyte));
            return tmpvalue;
        }

        public static short readShort(MemoryStream data)
        {
            short tmpvalue;

            byte[] nowData = data.GetBuffer();
            tmpvalue = BitConverter.ToInt16(nowData, (int)data.Position);
            tmpvalue = IPAddress.NetworkToHostOrder(tmpvalue);
            readStream(data, sizeof(short));
            return tmpvalue;
        }

        public static ushort readUShort(MemoryStream data)
        {
            ushort tmpvalue;

            byte[] nowData = data.GetBuffer();

            tmpvalue = BitConverter.ToUInt16(nowData, (int)data.Position);
            tmpvalue = (ushort)IPAddress.NetworkToHostOrder((short)tmpvalue);
            readStream(data, sizeof(short));

            return tmpvalue;
        }

        public static int readInt(MemoryStream data)
        {
            int tmpvalue;

            byte[] nowData = data.GetBuffer();
            tmpvalue = BitConverter.ToInt32(nowData, (int)data.Position);
            tmpvalue = IPAddress.NetworkToHostOrder(tmpvalue);
            readStream(data, sizeof(int));

            return tmpvalue;
        }

        public static uint readUInt(MemoryStream data)
        {
            uint tmpvalue;

            byte[] nowData = data.GetBuffer();
            tmpvalue = BitConverter.ToUInt32(nowData, (int)data.Position);
            tmpvalue = (uint)IPAddress.NetworkToHostOrder((int)tmpvalue);
            readStream(data, sizeof(int));

            return tmpvalue;
        }

        public static UInt64 readULong(MemoryStream data)
        {
            UInt64 tmpvalue = 0;

            byte[] nowData = data.GetBuffer();

            int _begin = (int)data.Position;
            int _end = (int)data.Position + sizeof(Int64) + 1;

            for (int i = _begin, j = sizeof(Int64) - 1; i < _end; i++, j--)
                tmpvalue += (UInt64)(nowData[i] * Math.Pow(256, j));
            readStream(data, sizeof(Int64));

            return tmpvalue;
        }

        public static string readString(MemoryStream data)
        {
            int Len = readShort(data);

            string desc = System.Text.Encoding.UTF8.GetString(data.GetBuffer(), (int)data.Position, Len);
            readStream(data, (uint)Len);

            return desc;
        }

        public static bool readBool(MemoryStream data)
        {
            byte _value = readUByte(data);
            if (_value > 0)
                return true;
            else
                return false;
        }

        public static void readLoopBool(MemoryStream data, List<bool> _list)
        {
            int Len = readShort(data);
            for (int i = 0; i < Len; i++) _list.Add(readBool(data));
        }

        public static void readLoopUByte(MemoryStream data, List<byte> _list)
        {
            int Len = readShort(data);
            for (int i = 0; i < Len; i++) _list.Add(readUByte(data));
        }

        public static void readLoopByte(MemoryStream data, List<sbyte> _list)
        {
            int Len = readShort(data);
            for (int i = 0; i < Len; i++) _list.Add(readByte(data));
        }

        public static void readLoopUShort(MemoryStream data, List<ushort> _list)
        {
            int Len = readShort(data);
            for (int i = 0; i < Len; i++) _list.Add(readUShort(data));
        }

        public static void readLoopShort(MemoryStream data, List<short> _list)
        {
            int Len = readShort(data);
            for (int i = 0; i < Len; i++) _list.Add(readShort(data));
        }

        public static void readLoopUInt(MemoryStream data, List<uint> _list)
        {
            int Len = readShort(data);
            for (int i = 0; i < Len; i++) _list.Add(readUInt(data));
        }

        public static void readLoopInt(MemoryStream data, List<int> _list)
        {
            int Len = readShort(data);
            for (int i = 0; i < Len; i++) _list.Add(readInt(data));
        }

        public static void readLoopString(MemoryStream data, List<string> _list)
        {
            int Len = readShort(data);
            for (int i = 0; i < Len; i++) _list.Add(readString(data));
        }
        public static void writeByte(MemoryStream data, sbyte tmpvalue)
        {
            byte[] tmp = new byte[1];
            tmp[0] = (byte)tmpvalue;
            data.Write(tmp, 0, 1);
        }

        public static void writeUByte(MemoryStream data, byte tmpvalue)
        {
            byte[] nowData = BitConverter.GetBytes(tmpvalue);
            data.Write(nowData, 0, sizeof(byte));
        }

        public static void writeShort(MemoryStream data, short tmpvalue)
        {
            tmpvalue = IPAddress.HostToNetworkOrder(tmpvalue);
            byte[] nowData = BitConverter.GetBytes(tmpvalue);
            data.Write(nowData, 0, sizeof(short));
        }

        public static void writeUShort(MemoryStream data, ushort tmpvalue)
        {
            tmpvalue = (ushort)IPAddress.HostToNetworkOrder((short)tmpvalue);
            byte[] nowData = BitConverter.GetBytes(tmpvalue);
            data.Write(nowData, 0, sizeof(ushort));
        }

        public static void writeInt(MemoryStream data, int tmpvalue)
        {
            tmpvalue = IPAddress.HostToNetworkOrder(tmpvalue);

            data.Write(BitConverter.GetBytes(tmpvalue), 0, sizeof(int));
        }

        public static void writeUInt(MemoryStream data, uint tmpvalue)
        {
            tmpvalue = (uint)IPAddress.HostToNetworkOrder((int)tmpvalue);
            byte[] nowData = BitConverter.GetBytes(tmpvalue);
            data.Write(nowData, 0, sizeof(uint));
        }

        public static void writeULong(MemoryStream data, UInt64 tmpvalue)
        {
            byte[] nowData = new byte[8];
            for (int i = 0; i < sizeof(UInt64); i++)
                nowData[i] = (byte)(tmpvalue / (Math.Pow(256, (sizeof(UInt64) - i - 1))));
            data.Write(nowData, 0, sizeof(UInt64));
        }

        public static void writeBool(MemoryStream data, bool tmpvalue)
        {
            byte _value = 0;
            if (tmpvalue) _value = 1;
            writeUByte(data, _value);
        }

        public static void WriteLoopBool(MemoryStream data, List<bool> list)
        {
            writeShort(data, (short)list.Count);
            foreach (bool ele in list) writeBool(data, ele);
        }

        public static void writeLoopUByte(MemoryStream data, List<byte> list)
        {
            writeShort(data, (short)list.Count);
            foreach (byte ele in list) writeUByte(data, ele);
        }

        public static void writeLoopByte(MemoryStream data, List<sbyte> list)
        {
            writeShort(data, (short)list.Count);
            foreach (sbyte ele in list) writeByte(data, ele);
        }

        public static void writeLoopUShort(MemoryStream data, List<ushort> list)
        {
            writeShort(data, (short)list.Count);
            foreach (ushort ele in list) writeUShort(data, ele);
        }

        public static void writeLoopShort(MemoryStream data, List<short> list)
        {
            writeShort(data, (short)list.Count);
            foreach (short ele in list) writeShort(data, ele);
        }

        public static void writeLoopUInt(MemoryStream data, List<uint> list)
        {
            if (list == null)
            {
                writeShort(data, 0);
                return;
            }
            writeShort(data, (short)list.Count);
            foreach (uint ele in list) writeUInt(data, ele);
        }

        public static void writeLoopInt(MemoryStream data, List<int> list)
        {
            writeShort(data, (short)list.Count);
            foreach (int ele in list) writeInt(data, ele);
        }

        public static void writeLoopString(MemoryStream data, List<string> list)
        {
            writeShort(data, (short)list.Count);
            foreach (string ele in list) writeString(data, ele);
        }

        public static void writeString(MemoryStream data, string _src)
        {
            MemoryStream byteString = new MemoryStream();
            byte[] nowData = Encoding.UTF8.GetBytes(_src);
            byteString.Write(nowData, 0, nowData.Length);

            writeShort(data, (short)nowData.Length);
            data.Write(byteString.GetBuffer(), 0, (int)byteString.Length);
        }
    }
}
