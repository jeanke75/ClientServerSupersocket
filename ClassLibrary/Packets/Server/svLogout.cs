using System;

namespace ClassLibrary.Packets.Server
{
    [Serializable]
    public class svLogout
    {
        public string Username { get; set; }
    }
}
