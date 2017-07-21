using System;
using System.IO;
using Game.ObjectPool;
using Net.Lua;
using UnityEngine;
using System.Text;

namespace Net.Protos.Proto
{
    public class RawPacket
    {
        private RecycleMS msg;
        private byte[] data;

        public ushort mAskUniqueID { get; private set; }
        private ushort mCMDID;
        public int mIsBack;

        public void Release()
        {
            if (msg != null)
            {
                msg.Release();
                msg = null;
            }

            mAskUniqueID = 0;
        }

        public RawPacket(LuaRawPacket luaRawPacket)
        {
            this.mCMDID = luaRawPacket.mCMDID;
            this.data = luaRawPacket.bytes;
        }

        public int GetMsgID()
        {
            return this.mCMDID;
        }

        public bool IsHeartPacket()
        {
            return mCMDID == MsgManager.cHeartPackageID;
        }

        public byte[] PackProtobuf(proto_util protoUtil)
        {
            if (msg == null)
            {
                msg = RecycleMS.CreateInstance();

                if (data != null)
                {
                    msg.Write(data, 0, data.Length);
                }
            }

            mAskUniqueID = Convert.ToUInt16(protoUtil.AskUniqueID());
            //Debug.LogError("mAskUniqueID:" + mAskUniqueID);

            ushort packageLen = Convert.ToUInt16(NetConstant.cPackageHeadSize + msg.Length);
            byte[] bytes = msg.ToArray();

            RecycleMS packageCrc = RecycleMS.CreateInstance();
            proto_util.writeUShort(packageCrc, mAskUniqueID);
            proto_util.EncryptPackage(packageCrc, bytes);
            byte[] crcBytes = packageCrc.ToArray();
            packageCrc.Release();

            RecycleMS package = RecycleMS.CreateInstance();
            package.SetLength(packageLen);

            proto_util.writeUShort(package, Convert.ToUInt16(msg.Length));
            proto_util.writeUShort(package, this.mCMDID);
            proto_util.writeUShort(package, mAskUniqueID);
            proto_util.writeUShort(package, CRC16Util.ConCRC(crcBytes, crcBytes.Length));

            proto_util.EncryptPackage(package, bytes);

            byte[] packBytes = package.ToArray();
            package.Release();//package在后面的代码不再使用，则释放

            return packBytes;
        }
    }
}
