using System;

namespace Shared.Packets.Server
{
    [Serializable]
    public class svLogout : BaseServerPacket
    {
        public string Username { get; set; }
    }
}