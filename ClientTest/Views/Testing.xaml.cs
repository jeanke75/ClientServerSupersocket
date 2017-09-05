using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using ClassLibrary.Maps;
using ClassLibrary.Models;
using ClassLibrary.Packets.Client;
using ClassLibrary.Packets.Enums;
using ClassLibrary.Packets.Server;
using ClientTest.Models;

namespace ClientTest.Views
{
    /// <summary>
    /// Interaction logic for Testing.xaml
    /// </summary>
    public sealed partial class Testing : UserControl, IDisposable
    {
        MainWindow main;

        Timer GameLoop = new Timer(100);

        Dictionary<string, BaseMap> Maps = new Dictionary<string, BaseMap>();

        ObservableQueue<svChat> chatQueue = new ObservableQueue<svChat>();
        public ObservableQueue<svChat> ChatQueue { get { return chatQueue; } }

        ObservableQueue<string> consoleQueue = new ObservableQueue<string>();
        public ObservableQueue<string> ConsoleQueue { get { return consoleQueue; } }

        Player hero = new Player();
        HashSet<Player> otherPlayers = new HashSet<Player>();

        public Testing(MainWindow main, svLogin login)
        {
            InitializeComponent();
            this.main = main;

            lbConsole.ItemsSource = ConsoleQueue;

            LoadMaps();

            GameLoop.Elapsed += new ElapsedEventHandler(GameLoop_Tick);
            GameLoop.Enabled = true;

            hero.Username = login.Username;
            hero.X = login.X;
            hero.Y = login.Y;
            hero.MapName = login.MapName;
            otherPlayers = login.Players;

            cMap.Height = Maps[hero.MapName].Height / 8;
            cMap.Width = Maps[hero.MapName].Width / 8;

            main.client.MovementMessageReceived += HandleMovement;
            main.client.LogoutMessageReceived += HandleLogout;

            lbChat.ItemsSource = ChatQueue;
            main.client.ChatMessageReceived += HandleChat;
        }

        public void Dispose()
        {
            GameLoop.Stop();
            GameLoop.Dispose();
            GC.SuppressFinalize(this);
        }

        private void LoadMaps()
        {
            Maps.Add("Town1", new BaseMap() { Height = 1600, Width = 1600 });
            Maps.Add("Wild1", new BaseMap() { Height = 800, Width = 2400 });
        }

        private void GameLoop_Tick(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)delegate ()
            {
                // update minimap
                cMap.Children.Clear();
                foreach (Player p in otherPlayers)
                {
                    Rectangle r = new Rectangle()
                    {
                        Height = 3,
                        Width = 3,
                        Fill = System.Windows.Media.Brushes.Yellow
                    };
                    cMap.Children.Add(r);
                    Canvas.SetLeft(r, p.X / 8);
                    Canvas.SetTop(r, p.Y / 8);
                }

                Rectangle rect = new Rectangle()
                {
                    Height = 3,
                    Width = 3,
                    Fill = System.Windows.Media.Brushes.Red
                };
                cMap.Children.Add(rect);
                Canvas.SetLeft(rect, hero.X / 8);
                Canvas.SetTop(rect, hero.Y / 8);

                cMap.InvalidateVisual();
            });
        }

        private void btnChat_Click(object sender, RoutedEventArgs e)
        {
            if (!main.client.IsConnected || txtChatInput.Text.TrimEnd() == "") return;

            string msg = txtChatInput.Text.TrimEnd();

            string[] splitMessage = msg.Split(' ');

            switch (splitMessage[0])
            {
                case "/w":
                case "/whisper":
                    if (splitMessage.Length >= 3)
                        main.client.SendMessage(new cChat() { Type = ChatTypes.Whisper, Message = string.Join(" ", splitMessage.Skip(2).ToArray()), Recipient = splitMessage[1] });
                    break;
                case "/p":
                case "/party":
                    main.client.SendMessage(new cChat() { Type = ChatTypes.Party, Message = string.Join(" ", splitMessage.Skip(1).ToArray()) });
                    break;
                case "/g":
                case "/guild":
                    main.client.SendMessage(new cChat() { Type = ChatTypes.Guild, Message = string.Join(" ", splitMessage.Skip(1).ToArray()) });
                    break;
                case "/s":
                case "/shout":
                    main.client.SendMessage(new cChat() { Type = ChatTypes.Server, Message = string.Join(" ", splitMessage.Skip(1).ToArray()) });
                    break;
                case "/bs":
                case "/bigshout":
                    main.client.SendMessage(new cChat() { Type = ChatTypes.All, Message = string.Join(" ", splitMessage.Skip(1).ToArray()) });
                    break;
                default:
                    main.client.SendMessage(new cChat() { Type = ChatTypes.Normal, Message = msg });
                    break;
            }
            txtChatInput.Text = "";
        }

        private void HandleChat(svChat chat)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                WriteToConsole("Chat message received");
                if (ChatQueue.Count() > 50) ChatQueue.Dequeue();
                ChatQueue.Enqueue(chat);
            });
        }

        private void btnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (!main.client.IsConnected) return;
            hero.Y -= (ushort)(hero.Y - 5 >= 0 ? 5 : hero.Y);
            main.client.SendMessage(new cMove() { X = hero.X, Y = hero.Y });
        }

        private void btnMoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (!main.client.IsConnected) return;
            hero.Y += (ushort)(hero.Y + 5 <= Maps[hero.MapName].Height ? 5 : hero.Y);
            main.client.SendMessage(new cMove() { X = hero.X, Y = hero.Y });
        }

        private void btnMoveLeft_Click(object sender, RoutedEventArgs e)
        {
            if (!main.client.IsConnected) return;
            hero.X -= (ushort)(hero.X - 5 >= 0 ? 5 : hero.X);
            main.client.SendMessage(new cMove() { X = hero.X, Y = hero.Y });
        }

        private void btnMoveRight_Click(object sender, RoutedEventArgs e)
        {
            if (!main.client.IsConnected) return;
            hero.X += (ushort)(hero.X + 5 < Maps[hero.MapName].Width ? 5 : hero.X);
            main.client.SendMessage(new cMove() { X = hero.X, Y = hero.Y });
        }

        private void HandleMovement(svMove move)
        {
            lbConsole.Dispatcher.Invoke(delegate
            {
                if (move.Success) // packet from other
                {
                    if (move.Username != null)
                    {
                        Player other = otherPlayers.Where(x => x.Username == move.Username).FirstOrDefault();
                        if (other != null) // known player
                        {
                            WriteToConsole(move.Username + " move (" + move.X + ", " + move.Y + ")");
                            other.X = move.X;
                            other.Y = move.Y;
                        }
                        else // new player
                        {
                            WriteToConsole(move.Username + " spawn (" + move.X + ", " + move.Y + ")");
                            otherPlayers.Add(new Player() { Username = move.Username, X = move.X, Y = move.Y });
                        }
                    }
                }
                else if (move.ErrorMessage != null)
                {
                    WriteToConsole("Something went wrong during movement: " + move.ErrorMessage);
                }
                else
                {
                    WriteToConsole("Invalid move! (cheating?!) Moved back to (" + move.X + ", " + move.Y + ")");
                    hero.X = move.X;
                    hero.Y = move.Y;
                }
            });
        }

        private void HandleLogout(svLogout logout)
        {
            lbConsole.Dispatcher.Invoke(delegate
            {
                WriteToConsole(logout.Username + " disconnected");
                otherPlayers.RemoveWhere(x => x.Username == logout.Username);
            });
        }

        private void WriteToConsole(string msg)
        {
            if (ConsoleQueue.Count() > 50) ConsoleQueue.Dequeue();
            ConsoleQueue.Enqueue(msg);
        }
    }
}
