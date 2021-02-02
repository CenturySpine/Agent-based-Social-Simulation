using System;
using System.Collections.Generic;
using System.Linq;
using Container = SimpleInjector.Container;

namespace SocialSimulation
{
    public class MovementService
    {
        private readonly GlobalSimulationParameters _simParams;
        private readonly Logger _logger;

        private readonly List<Func<IEntityBehavior>> _behaviors;
        private readonly Random _rnd = new Random(DateTime.Now.Millisecond);
        private Dictionary<Entity, MoveData> _goalTrack;

        public MovementService(GlobalSimulationParameters simParams, Container container, Logger logger)
        {
            _simParams = simParams;
            _logger = logger;
            _goalTrack = new Dictionary<Entity, MoveData>();
            _behaviors = new List<Func<IEntityBehavior>>
            {
                container.GetInstance<AudacityBehavior>,
                container.GetInstance<MoveBehavior>,
            };
        }

        public void Update(Entity entity)
        {
            foreach (var entityBehavior in _behaviors.Select(b => b()))
            {
                entityBehavior.Behave(entity, _simParams, _rnd, _goalTrack);
            }
        }
    }
}