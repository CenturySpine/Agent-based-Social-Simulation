using System;

namespace SocialSimulation
{
    public interface IEntityBehavior
    {
        void Behave(Entity entity, GlobalSimulationParameters simulationParams, Random random/*, Dictionary<Entity, MoveData> goalTrack*/);
    }
}