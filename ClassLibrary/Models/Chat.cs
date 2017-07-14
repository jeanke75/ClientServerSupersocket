using System;

namespace ClassLibrary.Models
{
    public enum ChatTypes
    {
        Whisper,
        Normal,
        Party,
        Guild,
        Server,
        All
    }

    [Serializable]
    public class Chat
    {
        public ChatTypes Type { get; set; }
        public string Message { get; set; }
        public string Recipient { get; set; }
    }
}
