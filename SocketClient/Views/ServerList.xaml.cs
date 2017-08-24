using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ClassLibrary;
using ClassLibrary.Models;
using SocketClient.Filters;
using SocketClient.Models;
using SuperSocket.ClientEngine;
using SuperSocket.ProtoBase;

namespace SocketClient.Views
{
    /// <summary>
    /// Interaction logic for ServerList.xaml
    /// </summary>
    public partial class ServerList : UserControl
    {
        public ObservableCollection<Server> servers { get; private set; }
        MainWindow mainWindow;

        public ServerList(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;

            servers = new ObservableCollection<Server>();
            servers.Add(new Server() { Type = Server.Types.Telnet, IP = "127.0.0.1", Port = 2012 });
            servers.Add(new Server() { Type = Server.Types.Custom, IP = "127.0.0.1", Port = 2013 });
            servers.Add(new Server() { Type = Server.Types.Custom, IP = "127.0.0.1", Port = 2014 });

            InitializeComponent();
        }

        private async void btnConnect_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Server sv = (Server)lvServers.SelectedItem;
            if (sv == null) return;

            switch (sv.Type)
            {
                case Server.Types.Custom:
                    Custom.ChatView cuc = new Custom.ChatView(new IPEndPoint(IPAddress.Parse(sv.IP), sv.Port));
                    mainWindow.SetContent(cuc);
                    await cuc.ConnectAsync();    
                    break;
                case Server.Types.Telnet:
                    Telnet.ChatView tuc = new Telnet.ChatView(new IPEndPoint(IPAddress.Parse(sv.IP), sv.Port));
                    mainWindow.SetContent(tuc);
                    await tuc.ConnectAsync();
                    break;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < int.Parse(txtAantal.Text); i++)
            {
                await Task.Factory.StartNew(() => NewClient(), TaskCreationOptions.LongRunning);
            }
        }

        private async void NewClient()
        {
            EasyClient client = new EasyClient();
            client.Connected += connected;
            client.Initialize(new TelnetReceiveFilter(Encoding.ASCII.GetBytes(Environment.NewLine)), MessageReceived);
            await client.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2013));
        }

        private void connected(object sender, EventArgs e)
        {
            SendMessage((EasyClient)sender, new Login() { username = new Random().Next(1000000, 2000000).ToString()});
            Thread.Sleep(1000);
            while (((EasyClient)sender).IsConnected)
            {
                int r = new Random().Next(500, 1000);
                Thread.Sleep(r);
                SendMessage((EasyClient)sender, new Chat() { Type = ChatTypes.Normal, Message = r + "ms" });
            }
        }

        private void MessageReceived(StringPackageInfo obj)
        {
        }

        private void SendMessage(EasyClient sender, object o)
        {
            Message msg = MessageHelper.Serialize(o);
            byte[] serializedMessage = MessageHelper.SerializeMessage(msg);

            byte[] header = Encoding.ASCII.GetBytes("##");
            byte[] rv = new byte[header.Length + serializedMessage.Length];
            Buffer.BlockCopy(header, 0, rv, 0, header.Length);
            Buffer.BlockCopy(serializedMessage, 0, rv, header.Length, serializedMessage.Length);

            sender.Send(rv);
        }
    }
}
