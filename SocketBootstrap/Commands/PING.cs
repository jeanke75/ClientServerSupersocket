using SocketServer.Models;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;

namespace SocketServer.Commands
{
    public class PING : CommandBase<TelnetSession, StringRequestInfo>
    {
        public override void ExecuteCommand(TelnetSession session, StringRequestInfo requestInfo)
        {
            session.Send("PONG!");
        }
    }
}
