using System;

namespace ClassLibrary.Packets.Client
{
    [Serializable]
    public class cRegister
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
