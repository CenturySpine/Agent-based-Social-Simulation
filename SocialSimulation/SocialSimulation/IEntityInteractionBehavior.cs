using System;

namespace SocialSimulation
{
    public interface IEntityInteractionBehavior
    {
        void Behave(Entity.Entity entitySource, Entity.Entity entityTarget, GlobalSimulationParameters simulationParams,
            Random random, InteractionService interactionService);
    }
}