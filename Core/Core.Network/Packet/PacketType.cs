namespace Core.Network.Packet
{
    public partial class PacketType : Enumeration
    {
        // public static readonly PacketType Example = new PacketType(1, "Example");

        public PacketType(int id, string name) 
            : base(id, name)
        {
        }
    }
}
