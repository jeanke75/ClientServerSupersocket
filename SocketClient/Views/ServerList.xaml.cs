using System.Collections.ObjectModel;
using System.Net;
using System.Windows.Controls;
using SocketClient.Models;

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
    }
}
