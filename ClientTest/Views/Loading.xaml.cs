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
            this.main.Title = GetTitle();
            main.client.SocketOpen += SocketConnected;
        }

        private void SocketConnected(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                main.SetContent(new LoginView(main));
            });
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i <= 5; i++)
            {
                if (await main.client.Start())
                    return;
                main.Title = GetTitle(i);
            }

            Application.Current.Dispatcher.Invoke(delegate {
                main.SetContent(new PickServer(main));
            });
        }

        private string GetTitle(int attempts = 0)
        {
            string title = $"Connecting to {main.client.Endpoint.ToString()}";
            if (attempts > 0)
            {
                title = $"{title} - failed attempts:{attempts}";
            }

            return title;
        }
    }
}