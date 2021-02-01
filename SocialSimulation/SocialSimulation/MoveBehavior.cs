using System;
using System.Windows;

namespace SocialSimulation
{
    public class MoveBehavior : IEntityBehavior
    {
        private Random _rnd = new Random(DateTime.Now.Millisecond);

        public void Behave(Entity entity, GlobalSimulationParameters simulationParams, Random random)
        {
            switch (entity.Direction)
            {
                case StartDirection.Left:
                    entity.Position = new Point(entity.Position.X - entity.Speed, entity.Position.Y);
                    if (entity.Position.X <= 0)
                    {
                        entity.Direction = StartDirection.Right;
                    }
                    break;

                case StartDirection.Top:
                    entity.Position = new Point(entity.Position.X, entity.Position.Y - entity.Speed);
                    if (entity.Position.Y <= 0)
                    {
                        entity.Direction = StartDirection.Bottom;
                    }
                    break;

                case StartDirection.Right:
                    entity.Position = new Point(entity.Position.X + entity.Speed, entity.Position.Y);
                    if (entity.Position.X >= simulationParams.SurfaceWidth - simulationParams.entitySize)
                    {
                        entity.Direction = StartDirection.Left;
                    }
                    break;

                case StartDirection.Bottom:
                    entity.Position = new Point(entity.Position.X, entity.Position.Y + entity.Speed);
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