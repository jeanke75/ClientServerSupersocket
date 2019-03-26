using Client;
using ClientTest.Models;
using Shared.Maps;
using Shared.Models;
using Shared.Packets.Client;
using Shared.Packets.Enums;
using Shared.Packets.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

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
        public ObservableQueue<svChat> ChatQueue { get; } = new ObservableQueue<svChat>();

        Player hero = new Player();
        HashSet<Player> otherPlayers = new HashSet<Player>();

        public Testing(MainWindow main, svLogin login)
        {
            InitializeComponent();
            this.main = main;

            LoadMaps();

            GameLoop.Elapsed += new ElapsedEventHandler(GameLoop_Tick);
            GameLoop.Enabled = true;

            hero.Username = login.Username;
            hero.X = login.X;
            hero.Y = login.Y;
            hero.MapName = login.MapName;
            otherPlayers = login.Players;

            ShowCurrentMap();

            main.client.MovementMessageReceived += HandleMovement;
            main.client.LogoutMessageReceived += HandleLogout;

            lbChat.ItemsSource = ChatQueue;
            main.client.ChatMessageReceived += HandleChat;

            main.client.TeleportAckMessageReceived += HandleTeleportAck;
            main.client.TeleportMessageReceived += HandleTeleport;
        }

        public void Dispose()
        {
            GameLoop.Stop();
            GameLoop.Dispose();
            GC.SuppressFinalize(this);
        }

        private void LoadMaps()
        {
            Maps.Add("Town1", new BaseMap( 1600, 1600 ));
            Maps.Add("Wild1", new BaseMap( 800, 2400 ));
            Maps.Add("Wild2", new BaseMap( 800, 800 ));
        }

        private void ShowCurrentMap()
        {
            cMap.Height = Maps[hero.MapName].Height / 8;
            cMap.Width = Maps[hero.MapName].Width / 8;
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
                        Fill = (p.Username.ToLower().Contains("bot") ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.Yellow)
                    };
                    cMap.Children.Add(r);
                    Canvas.SetLeft(r, p.X / 8);
                    Canvas.SetTop(r, p.Y / 8);
                }

                Rectangle rect = new Rectangle()
                {
                    Height = 3,
                    Width = 3,
                    Fill = System.Windows.Media.Brushes.Green
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
                case "/info":
                    ChatQueue.Enqueue(new svChat() { Type = ChatTypes.Error, Message = $"Up: {GameClient.bytesSent / 1000000}MB Down: {GameClient.bytesReceived / 1000000}MB \n Time running: {DateTime.Now - GameClient.startTime}" });
                    break;
                case "/tp":
                case "/teleport":
                    if (splitMessage.Length == 4)
                        main.client.SendMessage(new cTeleport() { MapName = splitMessage[1], X = ushort.Parse(splitMessage[2]), Y = ushort.Parse(splitMessage[3]) });
                    break;
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
                if (main.mniLogMessage.IsChecked)
                    main.log.WriteLine("Chat message received");
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
            Application.Current.Dispatcher.Invoke(delegate
            {
                if (move.Success) // packet from other
                {
                    if (move.Username != null && move.MapName != null && move.MapName == hero.MapName)
                    {
                        Player other = otherPlayers.Where(x => x.Username == move.Username).FirstOrDefault();
                        if (other != null) // known player
                        {
                            if (main.mniLogMovement.IsChecked)
                                main.log.WriteLine(move.Username + " move (" + move.X + ", " + move.Y + ")");
                            other.X = move.X;
                            other.Y = move.Y;
                        }
                        else // new player
                        {
                            if (main.mniLogSpawn.IsChecked)
                                main.log.WriteLine(move.Username + " spawn (" + move.X + ", " + move.Y + ")");
                            otherPlayers.Add(new Player() { Username = move.Username, X = move.X, Y = move.Y });
                        }
                    }
                }
                else if (move.ErrorMessage != null)
                {
                    main.log.WriteLine("Something went wrong during movement: " + move.ErrorMessage);
                }
                else
                {
                    main.log.WriteLine("Invalid move! (cheating?!) Moved back to (" + move.X + ", " + move.Y + ")");
                    hero.X = move.X;
                    hero.Y = move.Y;
                }
            });
        }

        private void HandleLogout(svLogout logout)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                if (main.mniLogOtherPlayerDisconnect.IsChecked)
                    main.log.WriteLine(logout.Username + " disconnected");
                otherPlayers.RemoveWhere(x => x.Username == logout.Username);
            });
        }

        private void HandleTeleportAck(svTeleport_ack teleport)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                if (teleport.Success)
                {
                    if (main.mniLogTeleport.IsChecked)
                        main.log.WriteLine("Teleported to " + teleport.MapName + "(" + teleport.X + ", " + teleport.Y + ")");
                    hero.X = teleport.X;
                    hero.Y = teleport.Y;
                    if (teleport.MapName != hero.MapName)
                    {
                        hero.MapName = teleport.MapName;
                        otherPlayers.Clear();
                        foreach (Player other in  teleport.Players)
                        {
                            otherPlayers.Add(other);
                        }
                        ShowCurrentMap();
                    }
                }
                else
                {
                    if (main.mniLogTeleport.IsChecked)
                        main.log.WriteLine("Teleport invalid");
                    ChatQueue.Enqueue(new svChat() { Type = ChatTypes.Error, Message = "Invalid teleport parameters" });
                }
            });
        }

        private void HandleTeleport(svTeleport teleport)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                if (main.mniLogTeleport.IsChecked)
                    main.log.WriteLine(teleport.Username + " teleported to " + teleport.MapName + "(" + teleport.X + ", " + teleport.Y + ")");

                if (teleport.Username != null)
                {
                    Player other = otherPlayers.Where(x => x.Username == teleport.Username).FirstOrDefault();
                    // known player 
                    if (other != null)
                    {
                        // from this map to this map
                        if (other.MapName == hero.MapName)
                        {
                            other.X = teleport.X;
                            other.Y = teleport.Y;
                        }
                        // from this map to other map
                        else
                        {
                            otherPlayers.RemoveWhere(x => x.Username == teleport.Username);
                        }
                    }
                    // unknown player
                    else
                    {
                        // from this map to this map
                        // from other map to this map
                        if (teleport.MapName == hero.MapName)
                        {
                            otherPlayers.Add(new Player() { Username = teleport.Username, X = teleport.X, Y = teleport.Y });
                        }
                    }
                }
            });
        }
    }
}