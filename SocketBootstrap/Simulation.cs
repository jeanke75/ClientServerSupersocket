using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using ClassLibrary;
using ClassLibrary.Maps;
using ClassLibrary.Packets.Server;
using SocketServer.Commands;
using SocketServer.Servers.Custom;

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
            while (!Plateform.window.Lifetime.Loop.IsExit)
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
            while (!Plateform.window.Lifetime.Loop.IsExit)
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
            while (!Plateform.window.Lifetime.Loop.IsExit)
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

            Random rnd = new Random();

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
                    ushort xtemp = (ushort)rnd.Next(0, 500);
                    ushort ytemp = (ushort)rnd.Next(0, 500);
                    server.GetAllSessions().Where(x => x.player != null && x.player.MapName == "Wild1")
                                 .AsParallel().ForAll(x => { PackageWriter.Write(x, new svMove() { Success = true, Username = "bot1", X = xtemp, Y = ytemp }); });
                    t += dt;
                    accumulator -= dt;
                }


                //const double alpha = accumulator / dt;

                //State state = currentState * alpha + previousState * (1.0 - alpha);

                //send updates to affected clients*/
            }
        }
        #endregion

        private ushort RandomMove(Random rnd, ushort curr, ushort max)
        {
            int move = rnd.Next(-5, 5);
            if (move < 0 && curr + move < 0) return (ushort)(curr - move);
            if (move > 0 && curr + move > max) return (ushort)(curr - move);
            return (ushort)(curr + move);
        }
    }
}