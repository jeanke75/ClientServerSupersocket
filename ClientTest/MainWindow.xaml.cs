using System.Windows;
using System.Windows.Controls;
using Client;
using ClientTest.Views;

namespace ClientTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public GameClient client;
        public MainWindow()
        {
            InitializeComponent();
            SetContent(new PickServer(this));
        }

        public void SetContent(UserControl c)
        {
            ccView.Content = c;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (client != null && client.IsConnected)
                client.Stop();
        }
    }
}
