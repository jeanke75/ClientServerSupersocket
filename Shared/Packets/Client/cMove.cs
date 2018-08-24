using System;

namespace Shared.Packets.Client
{
    [Serializable]
    public class cMove
    {
        public ushort X { get; set; }
        public ushort Y { get; set; }
    }
}