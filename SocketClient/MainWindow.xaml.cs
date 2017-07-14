using System.Windows;
using System.Windows.Controls;

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
    }
}
