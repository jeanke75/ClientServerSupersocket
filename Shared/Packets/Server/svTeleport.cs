using System;

namespace Shared.Packets.Server
{
    [Serializable]
    public class svTeleport : BaseServerPacket
    {
        public string MapName { get; set; }
        public string Username { get; set; }
        public ushort X { get; set; }
        public ushort Y { get; set; }
    }
}