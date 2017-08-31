using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary;
using ClassLibrary.Packets.Server;
using Client.Filters;
using SuperSocket.ClientEngine;

namespace Client
{
    public sealed class GameClient
    {
        EasyClient client;
        IPEndPoint endpoint;

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

        public GameClient(IPEndPoint endpoint)
        {
            client = new EasyClient();

            client.Connected += Connected;
            client.Closed += Closed;
            client.Error += Error;

            client.Initialize(new CustomReceiveFilter(header, footer), MessageReceived);

            this.endpoint = endpoint;
        }

        private void Connected(object sender, EventArgs e)
        {
            SocketOpen(sender, e);
        }

        private void Closed(object sender, EventArgs e)
        {
            SocketClosed(sender, e);
        }

        private void Error(object sender, ErrorEventArgs e)
        {
            SocketError(sender, e);
        }

        public async Task Start()
        {
            await client.ConnectAsync(endpoint);
        }

        public void Stop()
        {
            client.Close();
        }

        private void MessageReceived(CustomPackageInfo obj)
        {
            if (obj.Data is svMove) MovementMessageReceived(obj.Data as svMove);
            if (obj.Data is svChat) ChatMessageReceived(obj.Data as svChat);
            if (obj.Data is svSync) SyncMessageReceived(obj.Data as svSync);
            if (obj.Data is svLogin) LoginMessageReceived(obj.Data as svLogin);
            if (obj.Data is svLogout) LogoutMessageReceived(obj.Data as svLogout);
            if (obj.Data is svRegister) RegisterMessageReceived(obj.Data as svRegister);
        }

        public void SendMessage(object o)
        {
            Message msg = MessageHelper.Serialize(o);
            byte[] serializedMessage = MessageHelper.SerializeMessage(msg);

            byte[] rv = new byte[header.Length + serializedMessage.Length];
            Buffer.BlockCopy(header, 0, rv, 0, header.Length);
            Buffer.BlockCopy(serializedMessage, 0, rv, header.Length, serializedMessage.Length);

            client.Send(rv);
        }

        public bool IsConnected { get { return client.IsConnected; } }
        public IPEndPoint Endpoint { get { return endpoint; } }
    }
}
