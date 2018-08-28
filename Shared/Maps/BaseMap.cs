using System.Collections.Generic;
using Shared.Models;

namespace Shared.Maps
{
    public class BaseMap
    {
        public string Name { get; set; }
        public ushort Width { get; set; } = 100 * 32;
        public ushort Height { get; set; } = 100 * 32;
        public HashSet<Player> Players { get; set; } = new HashSet<Player>();
        public List<Player> Bots { get; set; } = new List<Player>();
    }
}