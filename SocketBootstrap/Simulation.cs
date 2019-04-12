using Shared;
using Shared.Maps;
using Shared.Packets.Server;
using SocketServer.Commands;
using SocketServer.Model;
using SocketServer.Servers.Custom;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace SocketServer
{
    public class Simulation
    {
        public volatile bool _IsRunning = false;

        #region Global Var
        public CustomServer server = null;
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
            SimulationThread = new Thread(_SimulationThreadStart)
            {
                IsBackground = true
            };
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
            double dt = 1000.0D / 30.0D;

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
                        b.Update();
                        /*server.GetAllSessions().Where(x => x.player != null && x.player.MapName == b.MapName)
                                .AsParallel().ForAll(x => { AddToQueue(x, new svMove() { Success = true, Username = b.Username, MapName = b.MapName, X = b.X, Y = b.Y }); });*/
                    }

                    t += dt;
                    accumulator -= dt;
                }

                var sessions = server.GetAllSessions().Where(x => x.player != null && x.player.MapName == map.Name);
                /* //only send updates for objects in a 200 x 200 square surrounding the player, should reduce package size alot!!! TODO: test package sizes
                var dirtyBots = bots.Where(x => x.isDirty);
                foreach (CustomSession s in sessions)
                {
                    foreach (Bot b in dirtyBots.Where(x => (x.X >= s.player.X - 100 && x.X <= s.player.X + 100) && (x.Y >= s.player.Y - 100 && x.Y <= s.player.Y + 100)))
                    {
                        AddToQueue(s, new svMove() { Success = true, Username = b.Username, MapName = b.MapName, X = b.X, Y = b.Y });
                    }
                }
                foreach (Bot b in dirtyBots)
                {
                    b.isDirty = false;
                }*/

                if (sessions.Count() > 0)
                {
                    foreach (Bot b in bots.Where(x => x.isDirty))
                    {
                        foreach (CustomSession s in sessions)
                        {
                            AddToQueue(s, new svMove() { Success = true, Username = b.Username, MapName = b.MapName, X = b.X, Y = b.Y });
                        }
                        b.isDirty = false;
                    }
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