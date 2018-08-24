using System.Collections.Generic;
using System.Linq;
using Shared.Maps;
using Shared.Models;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace SocketServer.Servers.Custom
{
    public class CustomServer : AppServer<CustomSession, CustomDataRequest>
    {
        public static HashSet<Player> Accounts = new HashSet<Player>()
        {
            new Player() { Username = "test", Password = "test", Email = "test", X = 800, Y = 800, MapName = "Town1" },
            new Player() { Username = "test1", Password = "test", Email = "test", X = 1200, Y = 400, MapName = "Wild1" },
            new Player() { Username = "test2", Password = "test", Email = "test", X = 1100, Y = 500, MapName = "Wild1" },
            new Player() { Username = "test3", Password = "test", Email = "test", X = 1000, Y = 600, MapName = "Wild1" },
            new Player() { Username = "test4", Password = "test", Email = "test", X = 900, Y = 700, MapName = "Wild1" },
            new Player() { Username = "test5", Password = "test", Email = "test", X = 800, Y = 800, MapName = "Wild1" }
        };

        public Simulation simulation;

        public CustomServer() : base(new DefaultReceiveFilterFactory<CustomReceiverFilter, CustomDataRequest>())
        {
            simulation = new Simulation(this);
            LoadMaps();
        }

        public override bool Start()
        {
            //simulation.Start();
            return base.Start();
        }

        public override void Stop()
        {
            if (simulation._IsRunning) simulation.Stop();
            //if (Simulation.IsRunning) simulation.Stop();
            //simulation.Stop();
            base.Stop();
        }

        public List<CustomServer> GetAllServersOfSameType()
        {
            return Bootstrap.AppServers.OfType<CustomServer>().ToList();
        }

        private void LoadMaps()
        {
            simulation.Maps.Add("Town1", new BaseMap() { Height = 1600, Width = 1600 });
            simulation.Maps.Add("Wild1", new BaseMap() { Height = 800, Width = 2400 });
        }
    }
}
