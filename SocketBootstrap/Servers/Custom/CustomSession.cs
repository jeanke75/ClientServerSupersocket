using System;
using System.Linq;
using ClassLibrary.Models;
using ClassLibrary.Packets.Server;
using SocketServer.Commands;
using SuperSocket.SocketBase;

namespace SocketServer.Servers.Custom
{
    public class CustomSession : AppSession<CustomSession, CustomDataRequest>
    {
        public Player player { get; set; }

        protected override void OnSessionStarted()
        {
            Console.WriteLine("{0}: Session created {1} from {2}", AppServer.Name, SessionID, RemoteEndPoint.Address.ToString());
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            // if player was logged in, send logout packet to every client on the same map
            if (player != null)
            {
                foreach (CustomSession otherSession in AppServer.GetAllSessions().Where(x => x.player != null && x.SessionID != SessionID && x.player.MapName == player.MapName))
                {
                    PackageWriter.Write(otherSession, new svLogout() { Username = player.Username });
                }

                Player p = CustomServer.Accounts.First(x => x.Username == player.Username);
                p.X = player.X;
                p.Y = player.Y;
                player = null;
            }

            Console.WriteLine("{0}: Session closed {1} ({2})", AppServer.Name, SessionID, reason);

            base.OnSessionClosed(reason);
        }
    }
}
