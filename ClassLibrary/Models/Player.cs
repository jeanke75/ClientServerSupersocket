using System;

namespace ClassLibrary.Models
{
    [Serializable]
    public class Player
    {
        public string Username { get; set; }
        [NonSerialized]
        public string Password;
        [NonSerialized]
        public string Email;

        public string MapName { get; set; }
        public ushort X { get; set; }
        public ushort Y { get; set; }
    }
}
