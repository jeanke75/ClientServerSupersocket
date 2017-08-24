using System;

namespace ClassLibrary.Packets.Server
{
    [Serializable]
    public class svRegister
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
