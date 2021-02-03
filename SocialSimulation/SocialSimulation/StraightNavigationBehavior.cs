using System;
using System.Numerics;

namespace SocialSimulation
{
    public class StraightNavigationBehavior : IDirectionInitiator
    {
        private readonly Logger _logger;

        public StraightNavigationBehavior(Logger logger)
        {
            _logger = logger;
        }

        public Vector2 InitiateDirectionGoal(Entity entity, GlobalSimulationParameters parameters)
        {
            Vector2 end;
            switch (entity.Direction)
            {
                case StartDirection.Left:
                    //move left = goal is the left border of surface, keeping top position constant
                    end = new Vector2(0, entity.Position.Y);
                    break;

                case StartDirection.Top:
                    //move top = goal is the top border of surface, keeping left position constant
                    end = new Vector2(entity.Position.X, 0);
                    break;

                case StartDirection.Right:
                    //move right = goal is the right border of surface, keeping top position constant
                    end = new Vector2(parameters.SurfaceWidth - parameters.entitySize, entity.Position.Y);
                    break;

                case StartDirection.Bottom:
                    //move bottom = goal is the bottom border of surface, keeping left position constant
                    end = new Vector2(entity.Position.X, parameters.SurfaceHeight - parameters.entitySize);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            _logger.Log($"Basic goal :{end}");
            _logger.Log($"Starting to move in straight line");
            return end;
        }
    }
}