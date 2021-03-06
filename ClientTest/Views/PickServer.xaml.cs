﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Client;

namespace ClientTest.Views
{
    /// <summary>
    /// Interaction logic for PickServer.xaml
    /// </summary>
    public sealed partial class PickServer : UserControl
    {
        MainWindow main;
        public PickServer(MainWindow main)
        {
            InitializeComponent();
            this.main = main;
            this.main.Title = "Connect to a server";
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            lblError.Visibility = Visibility.Hidden;
            try
            {
                main.client = new GameClient(new IPEndPoint(IPAddress.Parse(txtIP.Text), int.Parse(txtPort.Text)));
                main.SetContent(new Loading(main));
            }
            catch(Exception ex)
            {
                lblError.Content = ex.Message;
                lblError.Visibility = Visibility.Visible;
                main.client = null;
            }
        }
    }
}