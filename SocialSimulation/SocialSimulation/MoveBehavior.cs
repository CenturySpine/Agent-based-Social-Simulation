using System;
using System.Collections.Generic;
using System.Windows;

namespace SocialSimulation
{
    public class Logger
    {
        private readonly List<Action<string>> _listeners;
        private readonly object _listenersLock = new object();

        public Logger()
        {
            _listeners = new List<Action<string>>();
        }

        public void Log(string message)
        {
            lock (_listenersLock)
            {
                foreach (var listener in _listeners)
                {
                    listener(message);
                }
            }
        }

        public void RegisterListener(Action<string> listener)
        {
            lock (_listenersLock)
            {
                _listeners.Add(listener);
            }
        }
    }
    public class MoveBehavior : IEntityBehavior
    {
        private Random _rnd = new Random(DateTime.Now.Millisecond);
        private Logger _logger;

        public MoveBehavior(Logger logger)
        {
            _logger = logger;
        }
        public void Behave(Entity entity, GlobalSimulationParameters simulationParams, Random random)
        {

            if (AboutPointEqual(entity.Goal.GoalPosition, entity.Position))
            {
                MoveLinear(entity, simulationParams);
            }
            else
            {
                MoveTowardGoal(entity, simulationParams);
            }
        }
        public static bool AboutPointEqual(Point a, Point b)
        {
            return AboutEqual(a.X, b.X) && AboutEqual(a.Y, b.Y);
        }
        public static bool AboutEqual(double x, double y)
        {
            
            return Math.Abs(x - y) <= 0.001;
        }
        private void MoveTowardGoal(Entity entity, GlobalSimulationParameters simulationParams)
        {
            var goalPos = entity.Goal.GoalPosition;


            Vector totalTranslation = new Vector(goalPos.X - entity.Position.X /*- simulationParams.entitySize / 2*/, goalPos.Y - entity.Position.Y/* - simulationParams.entitySize / 2*/);

            //TODO : find the 
            Vector perSecond = totalTranslation / 16.666667;


            entity.Position = new Point(entity.Position.X + perSecond.X, entity.Position.Y + perSecond.Y);
            if (AboutPointEqual(entity.Goal.GoalPosition, entity.Position))
            {
                _logger.Log("Goal reached!!!");
            }
        }

        private static void MoveLinear(Entity entity, GlobalSimulationParameters simulationParams)
        {
            switch (entity.Direction)
            {
                case StartDirection.Left:
                    entity.Position = new Point(entity.Position.X - entity.Speed, entity.Position.Y);
                    entity.Goal.GoalPosition = entity.Position;
                    if (entity.Position.X <= 0)
                    {
                        entity.Direction = StartDirection.Right;
                    }

                    break;

                case StartDirection.Top:
                    entity.Position = new Point(entity.Position.X, entity.Position.Y - entity.Speed);
                    entity.Goal.GoalPosition = entity.Position;
                    if (entity.Position.Y <= 0)
                    {
                        entity.Direction = StartDirection.Bottom;
                    }

                    break;

                case StartDirection.Right:
                    entity.Position = new Point(entity.Position.X + entity.Speed, entity.Position.Y);
                    entity.Goal.GoalPosition = entity.Position;
                    if (entity.Position.X >= simulationParams.SurfaceWidth - simulationParams.entitySize)
                    {
                        entity.Direction = StartDirection.Left;
                    }

                    break;

                case StartDirection.Bottom:
                    entity.Position = new Point(entity.Position.X, entity.Position.Y + entity.Speed);
                    entity.Goal.GoalPosition = entity.Position;
                    if (entity.Position.Y >= simulationParams.SurfaceHeight - simulationParams.entitySize)
                    {
                        entity.Direction = StartDirection.Top;
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}