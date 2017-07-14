using System;
using System.Linq;
using ClassLibrary;
using ClassLibrary.Models;
using SocketServer.Servers.Custom;
using SuperSocket.SocketBase.Command;

namespace SocketServer.Commands.Custom
{
    public class LOGIN : CommandBase<CustomSession, CustomDataRequest>
    {
        public override void ExecuteCommand(CustomSession session, CustomDataRequest requestInfo)
        {
            if (session.userName != "")
            {
                session.Send("LOGINERR Already logged in!\r\n");
                return;
            }

            try
            {
                Login l = MessageHelper.Deserialize(requestInfo.Message) as Login;
                if (session.AppServer.GetAllSessions().Any(x => x.userName == l.username))
                {
                    session.Send("LOGINERR That username is already in use!\r\n");
                    return;
                }

                session.userName = l.username;
                session.Send("LOGIN Sucessfully logged in\r\n");
            }
            catch (Exception ex)
            {
                session.Send("LOGINERR " + ex.Message + " " + ex.GetType().ToString() + "\r\n");
            }
        }
    }
}
