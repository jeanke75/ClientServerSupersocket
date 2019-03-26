using Shared.Maps;
using SocketServer.Model;
using System;
using System.Collections.Generic;

namespace SocketServer.Data
{
    public interface IMapRepository
    {
        List<BaseMap> GetMaps();
        BaseMap GetMap(string name);
        List<Bot> GetBots(Simulation sim, Random random);
    }
}