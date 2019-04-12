using Client.Filters;
using Shared;
using Shared.Packets.Server;
using SuperSocket.ClientEngine;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public sealed class GameClient
    {
        EasyClient client;
        readonly byte[] header = Encoding.ASCII.GetBytes("##");
        readonly byte[] footer = Encoding.ASCII.GetBytes("$$");

        public event EventHandler SocketOpen;
        public event EventHandler SocketClosed;
        public event EventHandler<ErrorEventArgs> SocketError;

        public delegate void RegisterMessageDelegate(svRegister register);
        public RegisterMessageDelegate RegisterMessageReceived;
        public delegate void LoginMessageDelegate(svLogin login);
        public LoginMessageDelegate LoginMessageReceived;
        public delegate void LogoutMessageDelegate(svLogout logout);
        public LogoutMessageDelegate LogoutMessageReceived;

        public delegate void ChatMessageDelegate(svChat chat);
        public ChatMessageDelegate ChatMessageReceived;

        public delegate void SyncMessageDelegate(svSync sync);
        public SyncMessageDelegate SyncMessageReceived;

        public delegate void MovementMessageDelegate(svMove move);
        public MovementMessageDelegate MovementMessageReceived;

        public delegate void TeleportAckMessageDelegate(svTeleport_ack teleport);
        public TeleportAckMessageDelegate TeleportAckMessageReceived;

        public delegate void TeleportMessageDelegate(svTeleport teleport);
        public TeleportMessageDelegate TeleportMessageReceived;

        public static long bytesReceived = 0;
        public static long bytesSent = 0;
        public static DateTime startTime = DateTime.Now;

        public bool IsConnected { get { return client.IsConnected; } }
        public IPEndPoint Endpoint { get; }

        public GameClient(IPEndPoint endpoint)
        {
            client = new EasyClient();

            client.Connected += Connected;
            client.Closed += Closed;
            client.Error += Error;

            client.Initialize(new CustomReceiveFilter(header, footer), MessageReceived);

            Endpoint = endpoint;
        }

        private void Connected(object sender, EventArgs e)
        {
            SocketOpen?.Invoke(sender, e);
        }

        private void Closed(object sender, EventArgs e)
        {
            SocketClosed?.Invoke(sender, e);
        }

        private void Error(object sender, ErrorEventArgs e)
        {
            SocketError?.Invoke(sender, e);
        }

        public async Task<bool> Start()
        {
            return await client.ConnectAsync(Endpoint);
        }

        public void Stop()
        {
            client.Close();
        }

        private void MessageReceived(CustomPackageInfo obj)
        {
            if (obj.Data is svMulti)
            {
                foreach (BaseServerPacket p in ((svMulti)obj.Data).packets)
                {
                    HandleMessage(p);
                }
            }
            else
            {
                HandleMessage(obj.Data as BaseServerPacket);
            }
        }

        private void HandleMessage(BaseServerPacket command)
        {
            if (command is svMove) MovementMessageReceived?.Invoke(command as svMove);
            else if (command is svChat) ChatMessageReceived?.Invoke(command as svChat);
            else if (command is svSync) SyncMessageReceived?.Invoke(command as svSync);
            else if (command is svLogin) LoginMessageReceived?.Invoke(command as svLogin);
            else if (command is svLogout) LogoutMessageReceived?.Invoke(command as svLogout);
            else if (command is svRegister) RegisterMessageReceived?.Invoke(command as svRegister);
            else if (command is svTeleport_ack) TeleportAckMessageReceived?.Invoke(command as svTeleport_ack);
            else if (command is svTeleport) TeleportMessageReceived?.Invoke(command as svTeleport);
        }

        public void SendMessage(object o)
        {
            Message msg = MessageHelper.Serialize(o);
            byte[] serializedMessage = MessageHelper.SerializeMessage(msg);

            byte[] rv = new byte[header.Length + serializedMessage.Length];
            Buffer.BlockCopy(header, 0, rv, 0, header.Length);
            Buffer.BlockCopy(serializedMessage, 0, rv, header.Length, serializedMessage.Length);

            client.Send(rv);
            bytesSent += rv.Length;
        }
    }
}