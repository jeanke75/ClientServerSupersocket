using System;

namespace Shared
{
    [Serializable]
    public class Message
    {
        public string Key { get; set; }
        public byte[] Data { get; set; }
    }
}