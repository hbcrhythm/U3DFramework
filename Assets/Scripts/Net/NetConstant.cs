using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Net
{
    public static class NetConstant
    {
        public static readonly int cPackageHead_Len_Size = sizeof(short);//<<Len:16, Cmd:16,Crc:16,Seq:16>>
        public static readonly int cPakcageHead_CMD_CRC_SEQ_Size = sizeof(short) + sizeof(short) + sizeof(short);//<<Len:16, Cmd:16,Crc:16,Seq:16>>
        public static readonly int cPackageHeadSize = cPackageHead_Len_Size + cPakcageHead_CMD_CRC_SEQ_Size;//<<Len:16, Cmd:16,Crc:16,Seq:16>>
        public static readonly ushort cPackageCMDLenSize = sizeof(short) + sizeof(short);//包长度和协议号

        public const int cPackageCMDSize = sizeof(short);
        public const int cPackageLenSize = sizeof(short);//包长占2个字节
        public const int cUniqueSize = sizeof(short);//序列号占2个字节
        public const int cSessionModuleSize = sizeof(byte) * 2;//sessionID moduleID
    }
}
