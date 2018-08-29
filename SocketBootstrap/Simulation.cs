using Shared;
using Shared.Maps;
using Shared.Models;
using Shared.Packets.Server;
using SocketServer;
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
        public Random random = new Random();
        #endregion

        #region Accessors
        public Thread SimulationThread { get; private set; } = null;
        #endregion

        #region Game World
        public ConcurrentQueue<Message> packetsIn = new ConcurrentQueue<Message>();
        private Dictionary<CustomSession, Queue<BaseServerPacket>> packetsOut = new Dictionary<CustomSession, Queue<BaseServerPacket>>();
        public List<Bot> bots = new List<Bot>();
        public BaseMap map;
        #endregion

        #region Constructor

        /// <summary>
        ///
        /// </summary>
        public Simulation(CustomServer srv, BaseMap map)
        {
            server = srv;
            timer = new Stopwatch();
            this.map = map;

            _SimulationThreadStart = new ThreadStart(() => SimulationMethod());
        }

        public void Initialize()
        {
            bots = server.mapRepo.GetBots(this, random);
        }

        public void Start()
        {
            _IsRunning = true;
            timer.Start();
            SimulationThread = new Thread(_SimulationThreadStart);
            SimulationThread.Start();
        }

        public void Stop()
        {
            _IsRunning = false;
            timer.Stop();
        }

        #endregion

        #region Method
        private void SimulationMethod()
        {
            double t = 0.0;
            double dt = 1000.0D / 5.0D;

            long currentTime = timer.ElapsedMilliseconds;
            double accumulator = 0.0;

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
                        ushort xTmp = b.X;
                        ushort yTmp = b.Y;
                        b.DoMove();
                        if (b.X != xTmp || b.Y != yTmp)
                        {
                            server.GetAllSessions().Where(x => x.player != null && x.player.MapName == b.MapName)
                                 .AsParallel().ForAll(x => { AddToQueue(x, new svMove() { Success = true, Username = b.Username, X = b.X, Y = b.Y }); });
                        }
                    }

                    t += dt;
                    accumulator -= dt;
                }


                //const double alpha = accumulator / dt;

                //State state = currentState * alpha + previousState * (1.0 - alpha);

                //send updates to affected clients*/
                PackageWriter.FlushAll(packetsOut);
            }
        }

        public void AddToQueue(CustomSession session, BaseServerPacket obj)
        {
            Queue<BaseServerPacket> queue = null;
            if (!packetsOut.TryGetValue(session, out queue))
            {
                queue = new Queue<BaseServerPacket>();
                packetsOut.Add(session, queue);
            }

            queue.Enqueue(obj);
        }
        #endregion
    }
}

public class Bot : Player
{
    private static int IDCounter = 0;
    protected int moveSpeedMax = 5;
    protected Simulation simulation;

    public Bot(Simulation sim, Point spawn)
    {
        Username = "Bot" + IDCounter;
        IDCounter++;
        simulation = sim;
        MapName = sim.map.Name;
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

    public RoamBot(Simulation sim, Point spawn) : this(sim, spawn, 0) { }

    public RoamBot(Simulation sim, Point spawn, ushort roamRange) : base(sim, spawn)
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
                if (simulation.random.Next(0, 100) < 75)
                {
                    sleepCycles = simulation.random.Next(1, 10) * 5; // 5 = cycles per second (simulation rate)
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
        int moveX = simulation.random.Next(-roamRange, roamRange + 1);
        int moveY = simulation.random.Next(-roamRange, roamRange + 1);
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

    public PatrolBot(Simulation sim, Queue<Point> waypoints) : base(sim, waypoints.Peek())
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