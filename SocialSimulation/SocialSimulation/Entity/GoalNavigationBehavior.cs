using SocialSimulation.Movement;
using SocialSimulation.SimulationParameters;
using System.Linq;
using System.Numerics;

namespace SocialSimulation.Entity
{
    public class GoalNavigationBehavior : IDirectionInitiator
    {
        private readonly Logger _logger;

        public GoalNavigationBehavior(Logger logger)
        {
            _logger = logger;
        }

        public Vector2 InitiateDirectionGoal(SocialSimulation.Entity.Entity entity, GlobalSimulationParameters parameters)
        {
            if (entity.Goals.Any())
            {
                var g = entity.Goals.First();
                var end = new Vector2(g.GoalPosition.X, g.GoalPosition.Y);
                _logger.Log($"Defined goal :{end}");
                return end;
            }

            return Vector2.Zero;
        }
    }
}