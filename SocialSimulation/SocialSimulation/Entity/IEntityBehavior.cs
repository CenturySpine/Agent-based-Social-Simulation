using SocialSimulation.SimulationParameters;
using System;

namespace SocialSimulation.Entity
{
    public interface IEntityBehavior
    {
        void Behave(SocialSimulation.Entity.Entity entity, GlobalSimulationParameters simulationParams, Random random);
    }
}