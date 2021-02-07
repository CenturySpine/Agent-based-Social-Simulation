using SocialSimulation.SimulationParameters;
using System;

namespace SocialSimulation.Entity
{
    public interface IEntityBehavior
    {
        void Behave(Entity entity, GlobalSimulationParameters simulationParams, Random random, float elapsed);
    }
}