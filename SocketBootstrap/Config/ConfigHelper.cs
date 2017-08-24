using System.Configuration;

namespace SocketServer.Config
{
    public static class ConfigHelper
    {
        public static AccountConfig Account = ConfigurationManager.GetSection("gameSettings/account") as AccountConfig;
    }
}
