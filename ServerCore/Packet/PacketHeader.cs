using System;

namespace ServerCore.Packet
{
    public struct PacketHeader
    {
        public UInt16 Type;
        public UInt16 Size;

        public static UInt16 HeaderSize = sizeof(UInt16) + sizeof(UInt16);
    }
}
