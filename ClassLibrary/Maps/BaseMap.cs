using System.Collections.Generic;
using ClassLibrary.Models;

namespace ClassLibrary.Maps
{
    public class BaseMap
    {
        public ushort Width { get; set; } = 100 * 32;
        public ushort Height { get; set; } = 100 * 32;
        public HashSet<Player> Players { get; set; } = new HashSet<Player>();
    }
}
