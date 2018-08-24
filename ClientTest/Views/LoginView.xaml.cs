using System;
using System.Windows;
using System.Windows.Controls;
using Shared.Packets.Client;
using Shared.Packets.Server;

namespace ClientTest.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public sealed partial class LoginView : UserControl
    {
        MainWindow main;
        public LoginView(MainWindow main)
        {
            InitializeComponent();
            this.main = main;
            main.client.LoginMessageReceived += HandleLogin;
            main.client.RegisterMessageReceived += HandleRegister;
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            lblError.Visibility = Visibility.Hidden;
            try
            {
                main.client.SendMessage(new cRegister() { Username = txtUsernameRegister.Text, Password = txtPasswordRegister.Text, Email = txtEmailRegister.Text });
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            lblError.Visibility = Visibility.Hidden;
            try
            {
                main.client.SendMessage(new cLogin() { Username = txtUsernameLogin.Text, Password = txtPasswordLogin.Text });
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void HandleRegister(svRegister register)
        {
            if (register.Success)
            {
                Application.Current.Dispatcher.Invoke(delegate {
                    txtUsernameRegister.Text = "";
                    txtPasswordRegister.Text = "";
                    txtEmailRegister.Text = "";
                    tcTab.SelectedIndex = 0;
                });
            }
            else
            {
                ShowError(register.ErrorMessage);
            }
        }

        private void HandleLogin(svLogin login)
        {
            if (login.Success)
            {
                main.client.RegisterMessageReceived -= HandleRegister;
                main.client.LoginMessageReceived -= HandleLogin;
                Application.Current.Dispatcher.Invoke(delegate {
                    main.Title = string.Format("{0} - {1}", main.client.Endpoint.ToString(), login.Username);
                    main.SetContent(new Testing(main, login));
                });
            }
            else
            {
                ShowError(login.ErrorMessage);
            }
        }

        private void ShowError(string message)
        {
            lblError.Dispatcher.Invoke(delegate {
                lblError.Content = message;
                lblError.Visibility = Visibility.Visible;
            });
        }
    }
}
