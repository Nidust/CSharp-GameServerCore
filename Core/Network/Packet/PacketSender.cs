using Core.Network.Socket;
using System;
using System.Collections.Generic;
using System.IO;

namespace Core.Network.Packet
{
    public sealed class PacketSender : IDisposable
    {
        #region Properties
        private Queue<IPacket> mSendList;

        private Byte[] mSendBuffer;
        private UInt16 mSendBufferSize;
        private UInt16 mSentBufferSize;

        private MemoryStream mStream;
        #endregion

        #region Methods
        public PacketSender()
        {
            mSendList = new Queue<IPacket>();
        }

        public void Send(ClientSocket socket, IPacket packet)
        {
            mSendList.Enqueue(packet);

            if (IsSendingPacket() == false)
            {
                SendPacket(socket, GetPacketToSend());
            }
        }

        public bool Sending(ClientSocket socket, UInt16 sendBytes)
        {
            // 보내야 하는 사이즈를 넘어갔을 경우
            if (mSentBufferSize + sendBytes > mSendBufferSize)
            {
                throw new ArgumentOutOfRangeException($"Packet Sending Error");
            }

            mSentBufferSize += sendBytes;

            if (IsSendComplete() == false)
            {
                SendBuffer(socket);
                return false;
            }

            SendComplete(socket);
            return true;
        }
        #endregion

        #region Private
        private void SendComplete(ClientSocket socket)
        {
            mSendBuffer = null;
            Dispose();

            if (IsRemainPacketToSend())
            {
                SendPacket(socket, GetPacketToSend());
            }
        }

        private void SendPacket(ClientSocket socket, IPacket sendPacket)
        {
            if (sendPacket == null)
            {
                return;
            }

            mSendBuffer = PacketSerializer.Serailize(sendPacket);
            mSendBufferSize = (UInt16)mSendBuffer.Length;
            mSentBufferSize = 0;

            mStream = new MemoryStream(mSendBuffer);

            SendBuffer(socket);
        }

        private void SendBuffer(ClientSocket socket)
        {
            mStream.Seek(mSentBufferSize, SeekOrigin.Begin);

            Int32 remainBufferSize = mSendBufferSize - mSentBufferSize;
            if (remainBufferSize > socket.MaxSendBufferSize)
            {
                remainBufferSize = socket.MaxSendBufferSize;
            }

            ArraySegment<Byte> sendBuffer = new ArraySegment<Byte>(mStream.GetBuffer(), (Int32)mStream.Position, remainBufferSize);

            socket.Send(sendBuffer.Array);
        }

        private IPacket GetPacketToSend()
        {
            if (IsRemainPacketToSend() == false)
                return null;

            return mSendList.Dequeue();
        }

        private Boolean IsRemainPacketToSend()
        {
            return mSendList.Count > 0;
        }

        private Boolean IsSendingPacket()
        {
            return mSendBuffer != null;
        }

        private Boolean IsSendComplete()
        {
            return mSentBufferSize == mSendBufferSize;
        }

        public void Dispose()
        {
            if (mStream == null)
            {
                return;
            }

            mStream.Dispose();
        }
        #endregion
    }
}
