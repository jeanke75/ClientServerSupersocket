using System;
using Shared;
using Shared.Packets.Client;
using Shared.Packets.Server;
using SocketServer.Servers.Custom;
using SuperSocket.SocketBase.Command;

namespace SocketServer.Commands.Custom
{
    public class SYNC : CommandBase<CustomSession, CustomDataRequest>
    {
        public override string Name
        {
            get
            {
                return "c" + base.Name;
            }
        }
        public override void ExecuteCommand(CustomSession session, CustomDataRequest requestInfo)
        {
            cSync syncc = MessageHelper.Deserialize(requestInfo.Message) as cSync;
            PackageWriter.Write(session, new svSync() { clientTime = syncc.clientTime, serverTime = DateTime.Now });
        }
    }
}
