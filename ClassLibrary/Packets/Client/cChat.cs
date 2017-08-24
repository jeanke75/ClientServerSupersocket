using System;
using ClassLibrary.Packets.Enums;

namespace ClassLibrary.Packets.Client
{
    [Serializable]
    public class cChat
    {
        public ChatTypes Type { get; set; } = ChatTypes.Normal;
        public string Message { get; set; }
        public string Recipient { get; set; }
    }
}
