using System;

namespace ClassLibrary.Packets.Client
{
    [Serializable]
    public class cMove
    {
        public ushort X { get; set; }
        public ushort Y { get; set; }
    }
}
