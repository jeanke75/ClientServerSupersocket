using Client;
using ClientTest.Views;
using System.Windows;
using System.Windows.Controls;

namespace ClientTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static bool ApplicationClosing { get; private set; } = false;

        public GameClient client;
        public LogWindow Log { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            SetContent(new PickServer(this));
            Log = new LogWindow();
        }

        public void SetContent(UserControl c)
        {
            ccView.Content = c;
        }

        private void Window_StateChanged(object sender, System.EventArgs e)
        {
            if (Log != null)
                Log.WindowState = WindowState;
        }

        private void MenuLogStatus_Click(object sender, RoutedEventArgs e)
        {
            if (Log != null)
            {
                if (Log.Visibility != Visibility.Visible)
                {
                    Log.Show();
                }
                else
                {
                    Log.Hide();
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (client != null && client.IsConnected)
                client.Stop();
            if (Log != null)
                Log.ForceClose();
        }
    }
}