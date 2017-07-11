using System;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace SocketBootstrap.Models
{
    public class TelnetSession : AppSession<TelnetSession>
    {
        public string userName = "";
        protected override void OnSessionStarted()
        {
            this.Send("Welcome to SuperSocket Telnet Server - " + this.AppServer.Name);
        }

        protected override void HandleUnknownRequest(StringRequestInfo requestInfo)
        {
            this.Send("UNKWN");
        }

        protected override void HandleException(Exception e)
        {
            this.Send("APPERR: {0}", e.Message);
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            //add you logics which will be executed after the session is closed
            base.OnSessionClosed(reason);
        }
    }
}
