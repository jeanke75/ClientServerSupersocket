﻿using System;

namespace ClassLibrary.Packets.Server
{
    [Serializable]
    public class svMove
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string Username { get; set; }
        public ushort X { get; set; }
        public ushort Y { get; set; }
    }
}