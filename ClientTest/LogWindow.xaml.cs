using ClientTest.Models;
using System.Linq;
using System.Windows;

namespace ClientTest
{
    /// <summary>
    /// Interaction logic for Console.xaml
    /// </summary>
    public partial class LogWindow : Window
    {
        public ObservableQueue<string> LogQueue { get; } = new ObservableQueue<string>();
        private const uint MAX_LOG = 50;
        private bool closing = false;

        public LogWindow()
        {
            InitializeComponent();
            ShowInTaskbar = false;
            lbLog.ItemsSource = LogQueue;
        }

        public void WriteLine(string message)
        {
            if (Visibility == Visibility.Visible)
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    if (LogQueue.Count() > MAX_LOG) LogQueue.Dequeue();
                    LogQueue.Enqueue(message);
                });
            }
        }

        public void ForceClose()
        {
            closing = true;
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!closing)
            {
                e.Cancel = true;
                Hide();
            }
        }
    }
}