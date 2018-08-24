using Shared;
using SuperSocket.SocketBase.Protocol;

namespace SocketServer.Servers.Custom
{
    public class CustomDataRequest : IRequestInfo
    {
        public CustomDataRequest(string imei, Message message)
        {
            Key = imei;
            Message = message;
        }
        public string Key { get; private set; }
        public Message Message { get; private set; }
    }
}
