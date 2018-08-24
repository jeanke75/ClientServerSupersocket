using System;

namespace Shared.Packets.Server
{
    [Serializable]
    public class svRegister : BaseServerPacket
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}