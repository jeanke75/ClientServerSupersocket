using Shared.Models;
using System;
using System.Collections.Generic;

namespace Shared.Packets.Server
{
    [Serializable]
    public class svTeleport_ack : BaseServerPacket
    {
        public bool Success { get; set; }
        public string MapName { get; set; }
        public ushort X { get; set; }
        public ushort Y { get; set; }
        public List<Player> Players { get; set; }
    }
}