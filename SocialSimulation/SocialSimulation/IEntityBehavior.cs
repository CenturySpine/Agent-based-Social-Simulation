using System;

namespace SocialSimulation
{
    public interface IEntityBehavior
    {
        void Behave(Entity.Entity entity, GlobalSimulationParameters simulationParams, Random random);
    }
}