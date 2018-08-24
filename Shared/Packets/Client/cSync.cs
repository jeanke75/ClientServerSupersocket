using System;

namespace Shared.Packets.Client
{
    [Serializable]
    public class cSync
    {
        public DateTime clientTime { get; set; }
    }
}