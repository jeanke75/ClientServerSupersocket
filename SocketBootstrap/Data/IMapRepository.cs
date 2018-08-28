using Shared.Maps;
using System;
using System.Collections.Generic;

namespace SocketServer.Data
{
    public interface IMapRepository
    {
        List<BaseMap> GetMaps();
        List<Bot> GetBots(Simulation sim, Random random);
    }
}
