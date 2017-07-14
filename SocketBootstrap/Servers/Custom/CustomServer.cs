using System.Collections.Generic;
using System.Linq;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace SocketServer.Servers.Custom
{
    public class CustomServer : AppServer<CustomSession, CustomDataRequest>
    {
        public CustomServer() : base(new DefaultReceiveFilterFactory<CustomReceiverFilter, CustomDataRequest>())
        {
        }

        public List<CustomServer> GetAllServersOfSameType()
        {
            return this.Bootstrap.AppServers.OfType<CustomServer>().ToList();
        }
    }
}
