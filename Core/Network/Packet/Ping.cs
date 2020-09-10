using System;

namespace Core.Network.Packet
{
    public struct Ping : IPacket
    {
        public PacketHeader GetHedaer()
        {
            unsafe
            {
                PacketHeader header;
                header.Type = 0;
                header.Size = (UInt16)sizeof(Ping);

                return header;
            }
        }
    }

    public struct Pong : IPacket
    {
        public PacketHeader GetHedaer()
        {
            unsafe
            {
                PacketHeader header;
                header.Type = 0;
                header.Size = (UInt16)sizeof(Pong);

                return header;
            }
        }
    }
}
