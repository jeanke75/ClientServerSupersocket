using Shared.Models;
using System.Collections.Generic;

namespace SocketServer.Data.Hardcoded
{
    public class HardcodedPlayerRepository : IPlayerRepository
    {
        private HashSet<Player> players;

        public HardcodedPlayerRepository()
        {
            players = new HashSet<Player>()
            {
                new Player() { Username = "test", Password = "test", Email = "test", X = 800, Y = 800, MapName = "Town1" },
                new Player() { Username = "test1", Password = "test", Email = "test", X = 1200, Y = 400, MapName = "Wild1" },
                new Player() { Username = "test2", Password = "test", Email = "test", X = 1100, Y = 500, MapName = "Wild1" },
                new Player() { Username = "test3", Password = "test", Email = "test", X = 1000, Y = 600, MapName = "Wild1" },
                new Player() { Username = "test4", Password = "test", Email = "test", X = 900, Y = 700, MapName = "Wild1" },
                new Player() { Username = "test5", Password = "test", Email = "test", X = 100, Y = 100, MapName = "Wild2" }
            };
        }

        public HashSet<Player> GetAccounts()
        {
            return players;
        }
    }
}