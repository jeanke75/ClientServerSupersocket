using System;
using System.Collections.Generic;
using ClassLibrary.Models;

namespace ClassLibrary.Packets.Server
{
    [Serializable]
    public class svLogin
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        // user player data
        public string Username { get; set; }
        public ushort X { get; set; }
        public ushort Y { get; set; }
        public string MapName { get; set; }

        // all players on the same map
        public HashSet<Player> Players { get; set; }
    }
}
