using Shared.Packets.Enums;
using System;

namespace Shared.Packets.Server
{
    [Serializable]
    public class svChat : BaseServerPacket
    {
        public ChatTypes Type { get; set; } = ChatTypes.Normal;
        public string Message { get; set; }
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string Server { get; set; }
    }
}