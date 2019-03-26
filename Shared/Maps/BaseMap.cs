using System.Collections.Generic;
using Shared.Models;

namespace Shared.Maps
{
    public class BaseMap
    {
        public string Name { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public HashSet<Player> Players { get; set; } = new HashSet<Player>();
        public List<Player> Bots { get; set; } = new List<Player>();

        public BaseMap() : this("", 100 * 32, 100 * 32) { }

        public BaseMap(ushort Height, ushort Width) : this("", Height, Width) { }

        public BaseMap(string Name, ushort Height, ushort Width)
        {
            this.Name = Name;
            this.Height = Height;
            this.Width = Width;
        }
    }
}