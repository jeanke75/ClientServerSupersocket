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

                CustomServer serv = session.AppServer as CustomServer;

                bool success = serv.mapRepo.GetMaps().FirstOrDefault(x => x.Name == teleportc.MapName) != null;
                if (success)
                {
                    
                    string mapPrev = session.player.MapName;
                    bool mapchange = mapPrev != teleportc.MapName;

                    if (mapchange)
                    {
                        // start simulation loop on new map if it's not already running
                        serv.simulations.TryGetValue(teleportc.MapName, out Simulation sim);
                        if (!sim._IsRunning) sim.Start();
                    }

                    session.player.MapName = teleportc.MapName;
                    session.player.X = teleportc.X;
                    session.player.Y = teleportc.Y;

                    if (mapchange)
                    {
                        // stop simulation loop on old map if there are no other player on it
                        serv.simulations.TryGetValue(mapPrev, out Simulation sim);
                        if (sim._IsRunning && serv.GetAllSessions().Count(x => x.player != null && x.player.MapName == mapPrev) == 0) sim.Stop();
                    }

                    svTeleport_ack msg = new svTeleport_ack() { Success = true, MapName = teleportc.MapName, X = teleportc.X, Y = teleportc.Y, Players = new List<Player>() };

                    svTeleport teleports = new svTeleport() { MapName = teleportc.MapName, Username = session.player.Username, X = teleportc.X, Y = teleportc.Y };
                    foreach (CustomSession otherSession in serv.GetAllSessions().Where(x => x.player != null && x.SessionID != session.SessionID && (x.player.MapName == mapPrev || x.player.MapName == session.player.MapName)))
                    {
                        // send messages to people on the same map and on the destination map if it's different from the current
                        PackageWriter.Write(otherSession, teleports);

                        if (mapPrev != session.player.MapName && otherSession.player.MapName == teleportc.MapName)
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