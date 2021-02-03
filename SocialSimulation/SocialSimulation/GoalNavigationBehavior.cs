using System.Numerics;

namespace SocialSimulation
{
    public class GoalNavigationBehavior : IDirectionInitiator
    {
        private readonly Logger _logger;

        public GoalNavigationBehavior(Logger logger)
        {
            _logger = logger;
        }

        public Vector2 InitiateDirectionGoal(Entity entity, GlobalSimulationParameters parameters)
        {
            var end = new Vector2(entity.Goal.GoalPosition.X, entity.Goal.GoalPosition.Y);
            _logger.Log($"Defined goal :{end}");
            return end;
        }
    }
}