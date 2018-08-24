using System;
using System.Linq;
using Shared;
using Shared.Extensions;
using Shared.Models;
using Shared.Packets.Client;
using Shared.Packets.Server;
using SocketServer.Servers.Custom;
using SuperSocket.SocketBase.Command;

namespace SocketServer.Commands.Custom
{
    public class LOGIN : CommandBase<CustomSession, CustomDataRequest>
    {
        public override string Name
        {
            get
            {
                return "c" + base.Name;
            }
        }

        public override void ExecuteCommand(CustomSession session, CustomDataRequest requestInfo)
        {
            svLogin logins = new svLogin();
            try
            {
                cLogin loginc = MessageHelper.Deserialize(requestInfo.Message) as cLogin;

                // user and pw filled in?
                if (string.IsNullOrEmpty(loginc.Username?.Trim()) || string.IsNullOrEmpty(loginc.Password?.Trim()))
                {
                    PackageWriter.Write(session, new svLogin() { Success = false, ErrorMessage = "Please fill in a username and password." });
                    return;
                }

                // try to retrieve account based on credentials
                Player p = CustomServer.Accounts.Where(x => x.Username == loginc.Username && x.Password == loginc.Password).FirstOrDefault();

                // account found?
                if (p == null)
                {
                    PackageWriter.Write(session, new svLogin() { Success = false, ErrorMessage = "Username/password mismatch or the account doesn't exist." });
                    return;
                }
                
                // account logged in yet?
                if (((CustomServer)session.AppServer).GetAllServersOfSameType().Where(serv => serv.GetAllSessions().FirstOrDefault(x => x.player != null && x.player.Username == loginc.Username) != null).FirstOrDefault() != null)
                {
                    PackageWriter.Write(session, new svLogin() { Success = false, ErrorMessage = "The account is already logged in." });
                    return;
                }

                session.player = p;
                logins.Success = true;
                logins.Username = p.Username;
                logins.X = p.X;
                logins.Y = p.Y;
                logins.MapName = p.MapName;
                logins.Players = (from s in session.AppServer.GetAllSessions()
                                  where s.player != null && s.player.MapName == p.MapName && s.SessionID != session.SessionID
                                  select s.player).ToHashSet();

                // start simulation loop if it's not already running
                Simulation sim = (session.AppServer as CustomServer).simulation;
                if (!sim._IsRunning) sim.Start();

                // send succesfull login packet back to the client
                PackageWriter.Write(session, logins);

                session.AppServer.GetAllSessions().Where(x => x.player != null && x.SessionID != session.SessionID && x.player.MapName == p.MapName)
                                 .AsParallel().ForAll(x => { PackageWriter.Write(x, new svMove() { Success = true, Username = p.Username, X = p.X, Y = p.Y }); });

                // send data to all connected clients on the players map
                /*foreach (CustomSession otherSession in session.AppServer.GetAllSessions().Where(x => x.player != null && x.SessionID != session.SessionID && x.player.MapName == p.MapName))
                {
                    PackageWriter.Write(otherSession, new svMove() { Success = true, Username = p.Username, X = p.X, Y = p.Y });
                }*/
            }
            catch (Exception ex)
            {
                logins.Success = false;
                logins.ErrorMessage = ex.Message + " " + ex.GetType().ToString();
                PackageWriter.Write(session, logins);
            }
        }
    }
}
