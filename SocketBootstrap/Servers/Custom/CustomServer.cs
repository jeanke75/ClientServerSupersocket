using System.Collections.Generic;
using System.Linq;
using ClassLibrary.Maps;
using ClassLibrary.Models;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace SocketServer.Servers.Custom
{
    public class CustomServer : AppServer<CustomSession, CustomDataRequest>
    {
        public static HashSet<Player> Accounts = new HashSet<Player>()
        {
            new Player() { Username = "test", Password = "test", Email = "test", X = 800, Y = 800, MapName = "Town1" },
            new Player() { Username = "test1", Password = "test", Email = "test", X = 1200, Y = 400, MapName = "Wild1" }
        };

        public Dictionary<string, BaseMap> Maps = new Dictionary<string, BaseMap>();

        public CustomServer() : base(new DefaultReceiveFilterFactory<CustomReceiverFilter, CustomDataRequest>())
        {
            LoadMaps();
        }

        public List<CustomServer> GetAllServersOfSameType()
        {
            return Bootstrap.AppServers.OfType<CustomServer>().ToList();
        }

        private void LoadMaps()
        {
            Maps.Add("Town1", new BaseMap() { Height = 1600, Width = 1600 });
            Maps.Add("Wild1", new BaseMap() { Height = 800, Width = 2400 });
        }
    }
}
