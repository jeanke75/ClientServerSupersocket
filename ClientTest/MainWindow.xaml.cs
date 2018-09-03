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
        public LogWindow log { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            SetContent(new PickServer(this));
            log = new LogWindow();
        }

        public void SetContent(UserControl c)
        {
            ccView.Content = c;
        }

        private void Window_StateChanged(object sender, System.EventArgs e)
        {
            if (log != null)
                log.WindowState = WindowState;
        }

        private void mniLogStatus_Click(object sender, RoutedEventArgs e)
        {
            if (log != null)
            {
                if (log.Visibility != Visibility.Visible)
                {
                    log.Show();
                }
                else
                {
                    log.Hide();
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (client != null && client.IsConnected)
                client.Stop();
            if (log != null)
                log.ForceClose();
        }
    }
}