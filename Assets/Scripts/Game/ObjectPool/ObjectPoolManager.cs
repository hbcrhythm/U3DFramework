
using Net.Protos.Proto;
using UnityEngine;

namespace Game.ObjectPool
{
    class ObjectPoolManager
    {
        public static readonly ObjectPool<RecycleMS> sMemoryStreamPool = new ObjectPool<RecycleMS>();
        public static readonly ObjectPool<RawPacket> sRawPacketPool = new ObjectPool<RawPacket>();
    }
}
