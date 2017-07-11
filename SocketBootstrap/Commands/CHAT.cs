using System.Linq;
using SocketBootstrap.Models;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;

namespace SocketBootstrap.Commands
{
    public class CHAT : CommandBase<TelnetSession, StringRequestInfo>
    {
        public override void ExecuteCommand(TelnetSession session, StringRequestInfo requestInfo)
        {
            if (session.userName == "")
            {
                session.Send("LOGINERR Not logged in!");
                return;
            }

            switch (requestInfo.Parameters[0])
            {
                case "\\w": // whisper
                    if (requestInfo.Parameters.Length >= 3)
                    {
                        TelnetSession sOther = session.AppServer.GetAllSessions().Where(x => x.userName == requestInfo.Parameters[1]).FirstOrDefault();
                        if (sOther != null)
                        {
                            if (sOther.userName != session.userName)
                            {
                                string msg = string.Join(" ", requestInfo.Parameters.Skip(2).ToArray());
                                session.Send(string.Format("CHAT {0} -> {1}: {2}", session.userName, sOther.userName, msg));
                                sOther.Send(string.Format("CHAT {0} -> {1}: {2}", session.userName, sOther.userName, msg));
                            }
                            else
                            {
                                session.Send("CHATERR Can't whisper yourself!");
                            }
                        }
                        else
                        {
                            session.Send("CHATERR Didn't find the user " + requestInfo.Parameters[1] + ".");
                        }
                    }
                    else
                    {
                        session.Send("CHATERR Please provide a username and a message.");
                    }
                    break;
                default:
                    foreach (var s in session.AppServer.GetAllSessions())
                    {
                        s.Send("CHAT " + session.userName + ": " + requestInfo.Body);
                    }
                    break;
            }
        }
    }
}
