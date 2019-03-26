using System.Collections.Generic;
using System.Windows;

namespace SocketServer.Model
{
    public abstract class BotMovement
    {
        public abstract void Update(Simulation simulation, Bot bot);
    }

    public sealed class PatrolMovement : BotMovement
    {
        private Queue<Point> waypoints;

        public PatrolMovement(Queue<Point> waypoints)
        {
            this.waypoints = waypoints;
            NextWaypoint(); // put the first waypoint at the end because it's used as the initial spawn location
        }

        public override void Update(Simulation simulation, Bot bot)
        {
            Point p = waypoints.Peek();

            bot.MoveTowardsTarget((int)p.X, (int)p.Y);

            // if the target is close to the waypoint add the waypoint to the back of the queue
            if (bot.IsTargetInRange(p, bot.MoveSpeedMax))
                waypoints.Enqueue(waypoints.Dequeue());

            bot.isDirty = true;
        }

        private void NextWaypoint()
        {
            waypoints.Enqueue(waypoints.Dequeue());
        }
    }

    public sealed class RoamMovement : BotMovement
    {
        private readonly ushort centerX;
        private readonly ushort centerY;
        private readonly ushort roamRange;

        private Point waypoint;
        private int sleepCycles = 0;

        public RoamMovement(Simulation simulation, Point center, ushort roamRange)
        {
            centerX = (ushort)center.X;
            centerY = (ushort)center.Y;
            this.roamRange = roamRange;

            ResetWaypoint(simulation, center);
        }

        public override void Update(Simulation simulation, Bot bot)
        {
            if (sleepCycles == 0)
            {
                bot.MoveTowardsTarget((int)waypoint.X, (int)waypoint.Y);

                // check if the destination has been reached
                if (bot.IsTargetInRange(waypoint, roamRange))
                {
                    // choose a new destination
                    ResetWaypoint(simulation, new Point(bot.X, bot.Y));

                    // 75% chance to sleep for a period before moving again
                    if (simulation.random.Next(0, 100) < 75)
                    {
                        sleepCycles = simulation.random.Next(1, 10) * 5; // 5 = cycles per second (simulation rate)
                    }
                }

                bot.isDirty = true;
            }
            else
            {
                sleepCycles--;
            }
        }

        private void ResetWaypoint(Simulation simulation, Point currentPos)
        {
            int moveX = simulation.random.Next(-roamRange, roamRange + 1);
            int moveY = simulation.random.Next(-roamRange, roamRange + 1);
            int posX = (int)currentPos.X + moveX;
            int posY = (int)currentPos.Y + moveY;
            waypoint = new Point((ushort)ForceBounds(centerX - roamRange, centerX + roamRange, posX), (ushort)ForceBounds(centerY - roamRange, centerY + roamRange, posY));
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
}