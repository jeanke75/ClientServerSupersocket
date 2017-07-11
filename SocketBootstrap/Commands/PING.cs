using SocketBootstrap.Models;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;

namespace SocketBootstrap.Commands
{
    public class PING : CommandBase<TelnetSession, StringRequestInfo>
    {
        public override void ExecuteCommand(TelnetSession session, StringRequestInfo requestInfo)
        {
            session.Send("PONG!");
        }
    }
}
