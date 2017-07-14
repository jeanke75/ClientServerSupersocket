using System.Linq;
using SocketServer.Servers.Telnet;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;

namespace SocketServer.Commands.Telnet
{
    public class LOGIN : CommandBase<TelnetSession, StringRequestInfo>
    {
        public override void ExecuteCommand(TelnetSession session, StringRequestInfo requestInfo)
        {
            if (session.userName != "")
            {
                session.Send("LOGINERR Already logged in!");
                return;
            }

            if (session.AppServer.GetAllSessions().Any(x => x.userName == requestInfo.Parameters[0]))
            {
                session.Send("LOGINERR That username is already in use!");
                return;
            }

            session.userName = requestInfo.Parameters[0];
            session.Send("LOGIN Sucessfully logged in");
        }
    }
}
