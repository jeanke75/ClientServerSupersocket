using System;
using ClassLibrary.Packets.Enums;

namespace ClassLibrary.Packets.Server
{
    [Serializable]
    public class svChat
    {
        public ChatTypes Type { get; set; } = ChatTypes.Normal;
        public string Message { get; set; }
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string Server { get; set; }
    }
}
