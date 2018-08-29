using Shared;
using Shared.Models;
using Shared.Packets.Client;
using Shared.Packets.Server;
using SocketServer.Servers.Custom;
using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SocketServer.Commands.Custom
{
    public class TELEPORT : CommandBase<CustomSession, CustomDataRequest>
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
            // not logged in
            if (session.player == null) return;

            try
            {
                cTeleport teleportc = MessageHelper.Deserialize(requestInfo.Message) as cTeleport;
                bool success = (session.AppServer as CustomServer).mapRepo.GetMaps().FirstOrDefault(x => x.Name == teleportc.MapName) != null;
                if (success)
                {
                    bool mapchange = session.player.MapName != teleportc.MapName;

                    session.player.MapName = teleportc.MapName;
                    session.player.X = teleportc.X;
                    session.player.Y = teleportc.Y;

                    svTeleport_ack msg = new svTeleport_ack() { Success = true, MapName = teleportc.MapName, X = teleportc.X, Y = teleportc.Y, Players = new List<Player>() };

                    svTeleport teleports = new svTeleport() { MapName = teleportc.MapName, Username = session.player.Username, X = teleportc.X, Y = teleportc.Y };
                    foreach (CustomSession otherSession in session.AppServer.GetAllSessions().Where(x => x.player != null && x.SessionID != session.SessionID && (x.player.MapName == session.player.MapName || x.player.MapName == teleportc.MapName)))
                    {
                        // send messages to people on the same map and on the destination map if it's different from the current
                        PackageWriter.Write(otherSession, teleports);

                        if (mapchange && otherSession.player.MapName == teleportc.MapName)
                        {
                            msg.Players.Add(otherSession.player);
                        }
                    }

                    PackageWriter.Write(session, msg);
                }
                else
                {
                    PackageWriter.Write(session, new svTeleport_ack() { Success = false });
                }
            }
            catch (Exception)
            {
                Console.WriteLine("teleport failed");
            }
        }
    }
}