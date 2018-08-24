using System;

namespace Shared.Packets.Server
{
    [Serializable]
    public class svSync : BaseServerPacket
    {
        public DateTime clientTime { get; set; }
        public DateTime serverTime { get; set; }
    }
}