using Shared.Maps;
using SocketServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SocketServer.Data.Hardcoded
{
    public class HardcodedMapRepository : IMapRepository
    {
        private List<BaseMap> maps;

        public HardcodedMapRepository()
        {
            maps = new List<BaseMap>()
            {
                new BaseMap( "Town1", 1600, 1600 ),
                new BaseMap( "Wild1", 800, 2400 ),
                new BaseMap( "Wild2", 800, 800 )
            };
        }

        public List<Bot> GetBots(Simulation sim, Random random)
        {
            List<Bot> bots = new List<Bot>();
            switch (sim.map.Name)
            {
                case "Town1":
                    Queue<Point> waypoints3 = new Queue<Point>();
                    waypoints3.Enqueue(new Point(250, 100));
                    waypoints3.Enqueue(new Point(100, 400));
                    waypoints3.Enqueue(new Point(400, 400));
                    bots.Add(Bot.CreatePatrol(sim, waypoints3));

                    for (int i = 0; i < 50; i++)
                    {
                        bots.Add(Bot.CreateRoam(sim, new Point(random.Next(75, 1525), random.Next(75, 1525)), (ushort)random.Next(30, 75)));
                    }
                    break;
                case "Wild1":
                    Queue<Point> waypoints2 = new Queue<Point>();
                    waypoints2.Enqueue(new Point(25, 25));
                    waypoints2.Enqueue(new Point(475, 25));
                    waypoints2.Enqueue(new Point(475, 475));
                    waypoints2.Enqueue(new Point(25, 475));
                    bots.Add(Bot.CreatePatrol(sim, waypoints2));

                    for (int i = 0; i < 40; i++)
                    {
                        Queue<Point> waypoints = new Queue<Point>();
                        waypoints.Enqueue(new Point(50 + i * 50, 50));
                        waypoints.Enqueue(new Point(50 + i * 50, 500));
                        bots.Add(Bot.CreatePatrol(sim, waypoints));
                    }

                    for (int i = 0; i < 40; i++)
                    {
                        Queue<Point> waypoints = new Queue<Point>();
                        waypoints.Enqueue(new Point(75 + i * 50, 500));
                        waypoints.Enqueue(new Point(75 + i * 50, 50));
                        bots.Add(Bot.CreatePatrol(sim, waypoints));
                    }

                    for (int i = 0; i < 15; i++)
                    {
                        Queue<Point> waypoints = new Queue<Point>();
                        waypoints.Enqueue(new Point(50, 50 + i * 50));
                        waypoints.Enqueue(new Point(500, 50 + i * 50));
                        bots.Add(Bot.CreatePatrol(sim, waypoints));
                    }

                    for (int i = 0; i < 15; i++)
                    {
                        Queue<Point> waypoints = new Queue<Point>();
                        waypoints.Enqueue(new Point(500, 75 + i * 50));
                        waypoints.Enqueue(new Point(50, 75 + i * 50));
                        bots.Add(Bot.CreatePatrol(sim, waypoints));
                    }
                    break;
                case "Wild2":
                    for (int i = 0; i < 10; i++)
                    {
                        bots.Add(Bot.CreateRoam(sim, new Point(random.Next(100, 700), random.Next(100, 700)), 50, 100));
                    }
                    break;
            }

            return bots;
        }

        public List<BaseMap> GetMaps()
        {
            return maps;
        }

        public BaseMap GetMap(string name)
        {
            return maps.First(x => x.Name == name);
        }
    }
}