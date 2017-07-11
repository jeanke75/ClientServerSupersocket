using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;

namespace SocketBootstrap.Models
{
    public class TelnetServer : AppServer<TelnetSession>
    {
        protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
        {
            return base.Setup(rootConfig, config);
        }

        protected override void OnStartup()
        {
            base.OnStartup();
        }

        protected override void OnStopped()
        {
            base.OnStopped();
        }
    }
}
