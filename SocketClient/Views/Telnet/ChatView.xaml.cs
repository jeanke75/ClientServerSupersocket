using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SocketClient.Filters;
using SuperSocket.ClientEngine;
using SuperSocket.ProtoBase;

namespace SocketClient.Views.Telnet
{
    /// <summary>
    /// Interaction logic for ChatView.xaml
    /// </summary>
    public partial class ChatView : UserControl
    {
        EasyClient client;
        IPEndPoint endpoint;

        public ChatView(IPEndPoint endpoint)
        {
            InitializeComponent();

            this.endpoint = endpoint;

            client = new EasyClient();

            client.Error += Client_Error;
            client.Connected += Client_Connected;
            client.Closed += Client_Closed;

            client.Initialize(new TelnetReceiveFilter(Encoding.ASCII.GetBytes(Environment.NewLine)), MessageReceived);
        }

        #region client - server
        private void MessageReceived(StringPackageInfo spi)
        {
            Dispatcher.Invoke(delegate
            {
                switch (spi.Key)
                {
                    case "LOGIN":
                        dckLogin.Visibility = Visibility.Collapsed;
                        dckChat.Visibility = Visibility.Visible;
                        UpdateText(spi.Body);
                        break;
                    case "LOGINERR":
                    case "CHATERR":
                    case "CHAT":
                        UpdateText(spi.Body);
                        break;
                    default:
                        UpdateText(string.Format("{0} {1}", spi.Key, spi.Body));
                        break;
                }
            });
        }

        private void SendMessage(string key, string body)
        {
            client.Send(Encoding.ASCII.GetBytes(string.Format("{0} {1}\r\n", key, body)));
        }
        #endregion

        #region client events
        private void Client_Error(object sender, ErrorEventArgs e)
        {
            Dispatcher.Invoke(delegate
            {
                UpdateText(e.Exception.Message);
            });
        }

        private void Client_Connected(object sender, EventArgs e)
        {
            Dispatcher.Invoke(delegate
            {
                UpdateText("Connected");
                btnConnect.Visibility = Visibility.Collapsed;
                dckLogin.Visibility = Visibility.Visible;
            });
        }

        private void Client_Closed(object sender, EventArgs e)
        {
            Dispatcher.Invoke(delegate
            {
                UpdateText("Disconnected");
                btnConnect.Visibility = Visibility.Visible;
                dckLogin.Visibility = Visibility.Collapsed;
                dckChat.Visibility = Visibility.Collapsed;
            });
        }

        private async void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            await ConnectAsync();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (txtUsername.Text.Length < 5)
            {
                UpdateText("Username needs to be at least 5 characters long!");
                return;
            }
            SendMessage("LOGIN", txtUsername.Text);
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (!client.IsConnected || txtInput.Text.Trim() == "") return;
            SendMessage("CHAT", txtInput.Text.Trim());
            txtInput.Text = "";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (client != null && client.IsConnected)
                client.Close();
        }
        #endregion

        #region methods
        private void UpdateText(string message)
        {
            txtChat.AppendText(message + "\n");
        }

        public async Task ConnectAsync()
        {
            await client.ConnectAsync(endpoint);
        }
        #endregion
    }
}
