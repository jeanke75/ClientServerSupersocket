using System.Net;
using System.Windows;
using System.Windows.Controls;
using ClassLibrary.Models;
using Client;

namespace SocketClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetContent(new Views.ServerList(this));
        }

        public void SetContent(UserControl c)
        {
            ccPage.Content = c;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            GameClient c = new GameClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2014));
            c.ChatMessageReceived += test;
            await c.Start();
            c.SendMessage(new Chat() { Message = "test" });
        }

        private void test(Chat chat)
        {
            
        }
    }
}
