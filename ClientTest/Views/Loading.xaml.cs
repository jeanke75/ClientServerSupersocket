using System;
using System.Windows;
using System.Windows.Controls;

namespace ClientTest.Views
{
    /// <summary>
    /// Interaction logic for Loading.xaml
    /// </summary>
    public sealed partial class Loading : UserControl
    {
        MainWindow main;
        public Loading(MainWindow main)
        {
            InitializeComponent();
            this.main = main;
            main.client.SocketOpen += SocketConnected;
        }

        private void SocketConnected(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                main.Title = main.client.Endpoint.ToString();
                main.SetContent(new LoginView(main));
            });
        }

        private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            await main.client.Start();
        }
    }
}
