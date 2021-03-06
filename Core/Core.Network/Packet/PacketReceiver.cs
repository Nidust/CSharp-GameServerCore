﻿using System;

namespace Core.Network.Packet
{
    public sealed class PacketReceiver
    {
        #region Properties
        private Byte[] mBuffer;
        private Int32 mLength;

        private UInt16 mPacketSize;

        private static UInt16 PacketHeaderSize = sizeof(UInt16) + sizeof(UInt16);
        #endregion

        #region Methods
        public Boolean Receiving(Byte[] buffer, Int32 bufferLen, out IPacket receivePacket)
        {
            receivePacket = null;

            Int32 offSet = 0;
            Int32 length = bufferLen;

            if (IsReceivingPacket() == false)
            {
                mPacketSize = BitConverter.ToUInt16(buffer, 0);

                mBuffer = new Byte[mPacketSize];
                mLength = 0;

                offSet = PacketHeaderSize - 1;
                length -= PacketHeaderSize;
            }

            // 패킷 사이즈가 넘어갔을 떄
            if (mLength + length > mPacketSize)
            {
                throw new ArgumentOutOfRangeException($"Packet Receive Error (PacketSize={mPacketSize})");
            }

            Buffer.BlockCopy(buffer, offSet, mBuffer, mLength, length);
            mLength += length;

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
            packet = PacketSerializer.Deserialize(mBuffer);
            if (packet == null)
            {
                throw new NullReferenceException($"Packet Receive Error: packet is null");
            }

            mBuffer = null;

            return true;
        }

        private Boolean IsReceivingPacket()
        {
            return mBuffer != null;
        }

        private Boolean IsReceiveComplete()
        {
            return mLength == mPacketSize;
        }
        #endregion
    }
}
