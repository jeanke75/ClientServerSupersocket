using Shared.Models;
using SocketServer.Data;
using SocketServer.Data.Hardcoded;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SocketServer.Servers.Custom
{
    public class CustomServer : AppServer<CustomSession, CustomDataRequest>
    {
        public IPlayerRepository playerRepo = new HardcodedPlayerRepository();
        public IMapRepository mapRepo = new HardcodedMapRepository();

        public static HashSet<Player> Accounts;
        public static Random random = new Random();
        
        public Simulation simulation;

        public CustomServer() : base(new DefaultReceiveFilterFactory<CustomReceiverFilter, CustomDataRequest>())
        {
            /*foreach (BaseMap map in mapRepo.GetMaps())
            {
            }*/// TODO fix 1map per simulation and use dictionary to keep track of map simulation server
            simulation = new Simulation(this, mapRepo.GetMaps().First());
            simulation.Initialize();
            Accounts = playerRepo.GetAccounts();
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
    }
}
