using ServerCore.Packet;
using System;

namespace ServerCore.Session
{
    public sealed class PacketReceiver
    {
        #region Properties
        private Byte[] mBuffer;
        private Int32 mLength;

        private PacketHeader mPacketHeader;
        #endregion

        #region Methods
        public Boolean Receiving(Byte[] buffer, out IPacket receivePacket)
        {
            receivePacket = null;

            if (IsReceivingPacket() == false)
            {
                mPacketHeader.Type = BitConverter.ToUInt16(buffer, 0);
                mPacketHeader.Size = BitConverter.ToUInt16(buffer, sizeof(UInt16));

                mBuffer = new Byte[mPacketHeader.Size];
                mLength = 0;
            }

            // 패킷 사이즈가 넘어갔을 떄
            if (mLength + buffer.Length > mPacketHeader.Size)
            {
                throw new ArgumentOutOfRangeException($"Packet Receive Error (PacketSize={mPacketHeader.Size}, PacketType={mPacketHeader.Type})");
            }

            Buffer.BlockCopy(buffer, PacketHeader.HeaderSize, mBuffer, mLength, buffer.Length);

            if (IsReceiveComplete() == false)
            {
                return false;
            }

            return ReceiveComeplete(out receivePacket);
        }
        #endregion

        #region Private
        private bool ReceiveComeplete(out IPacket packet)
        {
            // TODO: Packet Deserializer 구현
            mBuffer = null;
            packet = null;

            return packet == null;
        }

        private Boolean IsReceivingPacket()
        {
            return mBuffer != null;
        }

        private Boolean IsReceiveComplete()
        {
            return mLength == mPacketHeader.Size;
        }
        #endregion
    }
}
