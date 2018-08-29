using System;

namespace Shared.Packets.Client
{
    [Serializable]
    public class cTeleport
    {
        public string MapName { get; set; }
        public ushort X { get; set; }
        public ushort Y { get; set; }
    }
}