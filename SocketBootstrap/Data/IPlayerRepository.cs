using Shared.Models;
using System.Collections.Generic;

namespace SocketServer.Data
{
    public interface IPlayerRepository
    {
        HashSet<Player> GetAccounts();
    }
}