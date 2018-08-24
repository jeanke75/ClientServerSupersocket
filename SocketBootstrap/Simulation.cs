using Shared;
using Shared.Maps;
using Shared.Models;
using Shared.Packets.Server;
using SocketServer.Commands;
using SocketServer.Servers.Custom;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;

namespace SocketServer
{
    public class Simulation
    {
        public volatile bool _IsRunning = false;

        #region Global Var
        private CustomServer server = null;
        private Stopwatch timer = null;

        private ThreadStart _SimulationThreadStart = null;
        private Thread _SimulationThread = null;

        public static Random random = new Random();
        #endregion

        #region Accessors
        public Thread SimulationThread
        {
            get
            {
                return _SimulationThread;
            }
        }
        #endregion

        #region Game World
        public Dictionary<string, BaseMap> Maps = new Dictionary<string, BaseMap>();
        public ConcurrentQueue<Message> packetsIn = new ConcurrentQueue<Message>();
        #endregion

        #region Constructor

        /// <summary>
        ///
        /// </summary>
        public Simulation(CustomServer srv)
        {
            server = srv;
            timer = new Stopwatch();

            _SimulationThreadStart = new ThreadStart(() => SimulationMethod());

            /*_RenderThreadStart = new ThreadStart(() => RenderMethod());
            _RenderThread = new Thread(_RenderThreadStart);

            _SoundThreadStart = new ThreadStart(() => SoundMethod());
            _SoundThread = new Thread(_SoundThreadStart);

            _ResourceThreadStart = new ThreadStart(() => ResourceMethod());
            _ResourceThread = new Thread(_ResourceThreadStart);

            _LogicThreadStart = new ThreadStart(() => LogicMethod());
            _LogicThread = new Thread(_LogicThreadStart);*/
        }

        public void Start()
        {
            _IsRunning = true;
            timer.Start();
            _SimulationThread = new Thread(_SimulationThreadStart);
            _SimulationThread.Start();
        }

        public void Stop()
        {
            _IsRunning = false;
            timer.Stop();
        }

        #endregion

        #region Method

        /*/// <summary>
        ///
        /// </summary>
        private void RenderMethod()
        {
            while (!Platform.window.Lifetime.Loop.IsExit)
            {
                Console.WriteLine("Render Method");
                Thread.Sleep(3000);
            }
        }

        /// <summary>
        ///
        /// </summary>
        private void SoundMethod()
        {
            while (!Platform.window.Lifetime.Loop.IsExit)
            {
                Console.WriteLine("Sound Method");
                Thread.Sleep(8000);
            }
        }

        /// <summary>
        ///
        /// </summary>
        private void ResourceMethod()
        {
            while (!Platform.window.Lifetime.Loop.IsExit)
            {
                Console.WriteLine("Resource Method");
                Thread.Sleep(6000);
            }
        }

        /// <summary>
        ///
        /// </summary>
        private void LogicMethod()
        {
            while (!Platform.window.Lifetime.Loop.IsExit)
            {
                Console.WriteLine("Logic Method");
                Thread.Sleep(13000);
            }
        }*/

        private async void SimulationMethod()
        {
            double t = 0.0;
            double dt = 1000.0D / 5.0D;

            long currentTime = timer.ElapsedMilliseconds;
            double accumulator = 0.0;

            List<Bot> bots = GenerateBots();

            while (_IsRunning)
            {
                long newTime = timer.ElapsedMilliseconds;
                long frameTime = newTime - currentTime;
                if (frameTime > 25)
                    frameTime = 25;
                currentTime = newTime;

                accumulator += frameTime;

                while (accumulator >= dt)
                {
                    //previousState = currentState;
                    //integrate(currentState, t, dt);

                    foreach (Bot b in bots)
                    {
                        b.DoMove();
                        server.GetAllSessions().Where(x => x.player != null && x.player.MapName == b.MapName)
                             .AsParallel().ForAll(x => { PackageWriter.AddToQueue(x, new svMove() { Success = true, Username = b.Username, X = b.X, Y = b.Y }); });
                    }

                    t += dt;
                    accumulator -= dt;
                }


                //const double alpha = accumulator / dt;

                //State state = currentState * alpha + previousState * (1.0 - alpha);

                //send updates to affected clients*/
                PackageWriter.FlushAll();
            }
        }
        #endregion

        public static List<Bot> GenerateBots()
        {
            List<Bot> bots = new List<Bot>();

            // Town1 bots
            Queue<Point> waypoints3 = new Queue<Point>();
            waypoints3.Enqueue(new Point(250, 100));
            waypoints3.Enqueue(new Point(100, 400));
            waypoints3.Enqueue(new Point(400, 400));
            bots.Add(new PatrolBot("Town1", waypoints3));

            for (int i = 0; i < 50; i++)
            {
                bots.Add(new RoamBot("Town1", new Point(random.Next(75, 1525), random.Next(75, 1525)), (ushort)random.Next(30, 75)));
            }

            // Wild1 bots
            Queue<Point> waypoints2 = new Queue<Point>();
            waypoints2.Enqueue(new Point(25, 25));
            waypoints2.Enqueue(new Point(475, 25));
            waypoints2.Enqueue(new Point(475, 475));
            waypoints2.Enqueue(new Point(25, 475));
            bots.Add(new PatrolBot("Wild1", waypoints2));

            for (int i = 0; i < 40; i++)
            {
                Queue<Point> waypoints = new Queue<Point>();
                waypoints.Enqueue(new Point(50 + i * 50, 50));
                waypoints.Enqueue(new Point(50 + i * 50, 500));
                bots.Add(new PatrolBot("Wild1", waypoints));
            }

            for (int i = 0; i < 40; i++)
            {
                Queue<Point> waypoints = new Queue<Point>();
                waypoints.Enqueue(new Point(75 + i * 50, 500));
                waypoints.Enqueue(new Point(75 + i * 50, 50));
                bots.Add(new PatrolBot("Wild1", waypoints));
            }

            for (int i = 0; i < 15; i++)
            {
                Queue<Point> waypoints = new Queue<Point>();
                waypoints.Enqueue(new Point(50, 50 + i * 50));
                waypoints.Enqueue(new Point(500, 50 + i * 50));
                bots.Add(new PatrolBot("Wild1", waypoints));
            }

            for (int i = 0; i < 15; i++)
            {
                Queue<Point> waypoints = new Queue<Point>();
                waypoints.Enqueue(new Point(500, 75 + i * 50));
                waypoints.Enqueue(new Point(50, 75 + i * 50));
                bots.Add(new PatrolBot("Wild1", waypoints));
            }

            return bots;
        }
    }

    public class Bot : Player
    {
        private static int IDCounter = 0;
        protected int moveSpeedMax = 5;

        public Bot(string mapName, Point spawn)
        {
            Username = "Bot" + IDCounter;
            IDCounter++;
            MapName = mapName;
            X = (ushort)spawn.X;
            Y = (ushort)spawn.Y;
        }

        public virtual void DoMove()
        {
            // do nothing, dumb bot
        }
    }

    public class RoamBot : Bot
    {
        private readonly ushort spawnX;
        private readonly ushort spawnY;
        private readonly ushort roamRange;
        private Point waypoint;
        private int sleepCycles = 0;

        public RoamBot(string mapName, Point spawn) : this(mapName, spawn, 0) { }

        public RoamBot(string mapName, Point spawn, ushort roamRange) : base(mapName, spawn)
        {
            spawnX = (ushort)spawn.X;
            spawnY = (ushort)spawn.Y;
            this.roamRange = roamRange;

            ResetWaypoint();
        }

        public override void DoMove()
        {
            if (sleepCycles == 0)
            {
                // calculate the distance
                int offsetX = (int)waypoint.X - X;
                int offsetY = (int)waypoint.Y - Y;

                // determine which distance is the furthest for the steps needed to reach the destination
                double steps = Math.Abs(Math.Round((Math.Abs(offsetX) > Math.Abs(offsetY) ? offsetX / (float)moveSpeedMax : offsetY / (float)moveSpeedMax)));

                // steps
                X += (ushort)Math.Round(offsetX / steps);
                Y += (ushort)Math.Round(offsetY / steps);

                // check if the destination has been reached
                if (Math.Abs(waypoint.X - X) <= moveSpeedMax && Math.Abs(waypoint.Y - Y) < moveSpeedMax)
                {
                    // choose a new destination
                    ResetWaypoint();

                    // 75% chance to sleep for a period before moving again
                    if (Simulation.random.Next(0, 100) < 75)
                    {
                        sleepCycles = Simulation.random.Next(1, 10) * 5; // 5 = cycles per second (simulation rate)
                    }
                }
            }
            else
            {
                sleepCycles--;
            }
        }

        private void ResetWaypoint()
        {
            int moveX = Simulation.random.Next(-roamRange, roamRange + 1);
            int moveY = Simulation.random.Next(-roamRange, roamRange + 1);
            int posX = X + moveX;
            int posY = Y + moveY;
            waypoint = new Point((ushort)ForceBounds(spawnX - roamRange, spawnX + roamRange, posX), (ushort)ForceBounds(spawnY - roamRange, spawnY + roamRange, posY));
        }

        private int ForceBounds(int lower, int upper, int current)
        {
            if (lower < 0) return 0;
            if (current < lower) return lower;
            if (upper > ushort.MaxValue) return ushort.MaxValue;
            if (current > upper) return upper;
            return current;
        }
    }

    public class PatrolBot : Bot
    {
        private Queue<Point> waypoints;

        public PatrolBot(string mapName, Queue<Point> waypoints) : base(mapName, waypoints.Peek())
        {
            waypoints.Enqueue(waypoints.Dequeue()); // put the first waypoint at the end because it's used as the initial spawn location
            this.waypoints = waypoints;
        }

        public override void DoMove()
        {
            Point p = waypoints.Peek();

            // calculate the distance
            int offsetX = (int)p.X - X;
            int offsetY = (int)p.Y - Y;

            // determine which distance is the furthest for the steps needed to reach the destination
            double steps = Math.Abs(Math.Round((Math.Abs(offsetX) > Math.Abs(offsetY) ? offsetX / (float)moveSpeedMax : offsetY / (float)moveSpeedMax)));

            // steps
            X += (ushort)Math.Round(offsetX / steps);
            Y += (ushort)Math.Round(offsetY / steps);

            // if the target is close to the waypoint add the waypoint to the back of the queue
            if (Math.Abs(p.X - X) <= moveSpeedMax && Math.Abs(p.Y - Y) < moveSpeedMax)
                waypoints.Enqueue(waypoints.Dequeue());
        }
    }
}