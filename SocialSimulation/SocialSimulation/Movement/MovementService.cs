using SocialSimulation.Entity;
using SocialSimulation.Interactions;
using SocialSimulation.SimulationParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using Container = SimpleInjector.Container;

namespace SocialSimulation.Movement
{
    public class MovementService
    {
        private readonly GlobalSimulationParameters _simParams;
        private readonly Logger _logger;
        private readonly InteractionService _interactService;

        private readonly List<Func<IEntityBehavior>> _behaviors;
        private readonly Random _rnd = new Random(DateTime.Now.Millisecond);

        public MovementService(GlobalSimulationParameters simParams, Container container, Logger logger, InteractionService interactService)
        {
            _simParams = simParams;
            _logger = logger;
            _interactService = interactService;

            _behaviors = new List<Func<IEntityBehavior>>
            {
                container.GetInstance<AudacityBehavior>,
                container.GetInstance<MoveBehavior>,
            };
        }

        public void Update(Entity.Entity entity, float elapsed)
        {
            foreach (var entityBehavior in _behaviors.Select(b => b()))
            {
                entityBehavior.Behave(entity, _simParams, _rnd, elapsed);
            }
        }
    }
}