using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Core.Network.Packet
{
    // TODO: 패킷 코드 자동 생성
    /*
    public partial class PacketType
    {
        public static PacketType SamplePacket = new PacketType(1, "SamplePacket");
    }

    public class SamplePacket : IPacket
    {
        public Int32 Value { get; private set; }

        public static void Serialize(BinaryWriter writer, IPacket packet)
        {
            SamplePacket thisPacket = packet as SamplePacket;

            writer.Write(thisPacket.Value);
        }

        public static IPacket Deserialize(BinaryReader reader)
        {
            SamplePacket result = new SamplePacket();

            result.Value = reader.ReadInt32();

            return result;
        }
    }
    */

    public static class PacketSerializer
    {
        #region Properties
        private static readonly Dictionary<String, Int32> PacketEnums;
        private static readonly Dictionary<Int32, Action<BinaryWriter, IPacket>> PacketWriters;
        private static readonly Dictionary<Int32, Func<BinaryReader, IPacket>> PacketReaders;
        #endregion

        #region Methods
        static PacketSerializer()
        {
            PacketEnums = Enumeration.GetAll<PacketType>().ToDictionary(key => key.Name, value => value.Id);

            Type[] packetTypes = typeof(IPacket)
                .Assembly
                .GetTypes()
                .Where(type => typeof(IPacket).IsAssignableFrom(type) && type.IsAbstract == false)
                .ToArray();

            PacketWriters = packetTypes
                .ToDictionary(
                    key => PacketEnums[key.Name],
                    value => (Action<BinaryWriter, IPacket>)value.GetMethod("Serialize").CreateDelegate(typeof(Action<BinaryWriter, IPacket>))
                );

            PacketReaders = packetTypes
                .ToDictionary(
                    key => PacketEnums[key.Name],
                    value => (Func<BinaryReader, IPacket>)value.GetMethod("Deserialize").CreateDelegate(typeof(Func<BinaryReader, IPacket>))
                );
        }

        public static Byte[] Serailize(IPacket packet)
        {
            String packetName = packet.GetType().Name;

            if (PacketEnums.ContainsKey(packetName) == false)
            {
                return null; // 등록되지 않은 Packet
            }

            Int32 packetEnum = PacketEnums[packetName];

            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write((UInt16)0); // packetSize
                writer.Write((UInt16)packetEnum); // packetId

                PacketWriters[packetEnum].Invoke(writer, packet);

                WriteAtPosition(writer, 0, (UInt16)writer.BaseStream.Position);

                return stream.GetBuffer();
            }
        }

        public static IPacket Deserialize(Byte[] buffer)
        {
            using (MemoryStream stream = new MemoryStream(buffer))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                reader.ReadUInt16(); // size
                UInt16 packetId = reader.ReadUInt16();

                if (PacketEnums.ContainsValue(packetId) == false)
                {
                    return null;
                }

                return PacketReaders[packetId].Invoke(reader);
            }
        }
        #endregion

        #region Private
        private static void WriteAtPosition(BinaryWriter writer, Int64 position, UInt16 value)
        {
            Int64 backup = writer.BaseStream.Position;

            writer.BaseStream.Seek(position, SeekOrigin.Begin);
            writer.Write(value);

            writer.BaseStream.Seek(backup, SeekOrigin.Begin);
        }
        #endregion
    }
}
