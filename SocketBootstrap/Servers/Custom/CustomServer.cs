using Shared.Maps;
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

        public Dictionary<string, Simulation> simulations = new Dictionary<string, Simulation>();

        public CustomServer() : base(new DefaultReceiveFilterFactory<CustomReceiverFilter, CustomDataRequest>())
        {
            foreach (BaseMap map in mapRepo.GetMaps())
            {
                Simulation simulation = new Simulation(this, map);
                simulation.Initialize();
                simulations.Add(map.Name, simulation);
            }
            
            Accounts = playerRepo.GetAccounts();
        }

        public override bool Start()
        {
            return base.Start();
        }

        public override void Stop()
        {
            foreach (Simulation sim in simulations.Values)
            {
                if (sim._IsRunning) sim.Stop();
            }
            base.Stop();
        }

        public List<CustomServer> GetAllServersOfSameType()
        {
            return Bootstrap.AppServers.OfType<CustomServer>().ToList();
        }
    }
}