using Shared.Packets.Enums;
using System;

namespace Shared.Packets.Client
{
    [Serializable]
    public class cChat
    {
        public ChatTypes Type { get; set; } = ChatTypes.Normal;
        public string Message { get; set; }
        public string Recipient { get; set; }
    }
}