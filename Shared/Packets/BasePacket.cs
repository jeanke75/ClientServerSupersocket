using Shared.Packets.Enums;

namespace Shared.Packets
{
    abstract class BasePacket
    {
        public abstract PacketType Key { get; }

        public abstract byte[] Serialize();
    }
}