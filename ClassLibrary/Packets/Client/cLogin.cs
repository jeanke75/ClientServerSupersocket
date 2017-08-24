using System;

namespace ClassLibrary.Packets.Client
{
    [Serializable]
    public class cLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
