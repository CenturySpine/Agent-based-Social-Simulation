using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Container = SimpleInjector.Container;

namespace SocialSimulation
{
    public class MovementService
    {
        private readonly GlobalSimulationParameters _simParams;

        private List<Func<IEntityBehavior>> behaviors;
        private Random _rnd = new Random(DateTime.Now.Millisecond);
        public MovementService(GlobalSimulationParameters simParams, Container container)
        {
            _simParams = simParams;


            behaviors = new List<Func<IEntityBehavior>>
            {
                container.GetInstance<AudacityBehavior>,
                container.GetInstance<MoveBehavior>,
            };
        }
        public void Update(Entity entity)
        {
            foreach (var entityBehavior in behaviors.Select(b => b()))
            {
                entityBehavior.Behave(entity, _simParams, _rnd);
            }
        }
    }
}