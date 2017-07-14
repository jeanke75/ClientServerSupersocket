using System;

namespace ClassLibrary
{
    [Serializable]
    public class Message
    {
        public string Key { get; set; }
        public byte[] Data { get; set; }
    }
}
