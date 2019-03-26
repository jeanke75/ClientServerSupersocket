using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SocketServer.Model
{
    public class Bot : Player
    {
        private static int IDCounter = 0;
        protected Simulation simulation;

        public Point Spawn { get; private set; }

        private BotMovement movement;
        public int MoveSpeedMax { get; private set; } = 5;

        private readonly ushort aggroRange;
        private Player target;

        public bool isDirty = false;

        private Bot(Simulation sim, Point spawn, BotMovement movement, ushort aggroRange)
        {
            Username = "Bot" + IDCounter;
            IDCounter++;
            simulation = sim;
            MapName = sim.map.Name;
            Spawn = spawn;
            X = (ushort)spawn.X;
            Y = (ushort)spawn.Y;
            this.movement = movement;
            this.aggroRange = aggroRange;
        }

        public static Bot CreateStatic(Simulation sim, Point spawn)
        {
            return new Bot(sim, spawn, null, 0);
        }

        public static Bot CreatePatrol(Simulation sim, Queue<Point> waypoints, ushort aggroRange = 0)
        {
            return new Bot(sim, waypoints.Peek(), new PatrolMovement(waypoints), aggroRange);
        }

        public static Bot CreateRoam(Simulation sim, Point spawn, ushort roamRange, ushort aggroRange = 0)
        {
            return new Bot(sim, spawn, new RoamMovement(sim, spawn, roamRange), (aggroRange >= roamRange ? aggroRange : roamRange));
        }

        public void Update()
        {
            if (aggroRange > 0)
            {
                // check if we have a target, otherwise try to find a new target
                if (target == null)
                {
                    target = simulation.server.GetAllSessions().FirstOrDefault(x => x.player != null && x.player.MapName == simulation.map.Name && IsTargetInAggroRange(x.player))?.player;
                }

                // if there is a target go to it, otherwise roam around
                if (target != null)
                {
                    // check if the target has not teleported and is still within aggro range
                    if (target.MapName == simulation.map.Name && IsTargetInAggroRange(target))
                    {
                        // check if target has been reached, if not move towards it otherwise do nothing
                        if (Math.Abs(target.X - X) >= MoveSpeedMax || Math.Abs(target.Y - Y) >= MoveSpeedMax)
                        {
                            MoveTowardsTarget(target.X, target.Y);

                            isDirty = true;
                        }
                    }
                    else
                    {
                        // if the target can't be reached remove the target
                        target = null;
                    }
                }
                else
                {
                    movement?.Update(simulation, this);
                }
            }
            else
            {
                movement?.Update(simulation, this);
            }
        }

        public void MoveTowardsTarget(int targetX, int targetY)
        {
            // calculate the distance
            int offsetX = targetX - X;
            int offsetY = targetY - Y;

            // determine which distance is the furthest for the steps needed to reach the destination
            double steps = Math.Abs(Math.Round((Math.Abs(offsetX) > Math.Abs(offsetY) ? offsetX / (float)MoveSpeedMax : offsetY / (float)MoveSpeedMax)));

            // steps
            X += (ushort)Math.Round(offsetX / steps);
            Y += (ushort)Math.Round(offsetY / steps);
        }

        public bool IsTargetInRange(Point target, int range)
        {
            return Math.Abs(target.X - X) <= range && Math.Abs(target.Y - Y) < range;
        }

        private bool IsTargetInAggroRange(Player p)
        {
            return IsValueInRange(p.X, (int)Spawn.X - aggroRange, (int)Spawn.X + aggroRange) && IsValueInRange(p.Y, (int)Spawn.Y - aggroRange, (int)Spawn.Y + aggroRange);
        }

        private bool IsValueInRange(int value, int min, int max)
        {
            return (value >= min && value <= max);
        }
    }
}