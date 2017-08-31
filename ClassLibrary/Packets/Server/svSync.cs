using System;

namespace ClassLibrary.Packets.Server
{
    [Serializable]
    public class svSync
    {
        public DateTime clientTime { get; set; }
        public DateTime serverTime { get; set; }
    }
}
