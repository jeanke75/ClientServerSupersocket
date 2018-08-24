using System;
using System.Collections.Generic;

namespace Shared.Packets.Server
{
    [Serializable]
    public class svMulti : BaseServerPacket
    {
        public List<BaseServerPacket> packets = new List<BaseServerPacket>();
    }
}