using SuperSocket.SocketBase;

namespace SocketServer.Servers.Custom
{
    public class CustomSession : AppSession<CustomSession, CustomDataRequest>
    {
        public string userName = "";
        protected override void OnSessionStarted()
        {
            this.Send("Welcome to SuperSocket Custom Server - " + this.AppServer.Name + "\r\n");
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            //add you logics which will be executed after the session is closed
            base.OnSessionClosed(reason);
        }
    }
}
