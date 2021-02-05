using System;

namespace SocialSimulation
{
    public interface IEntityInteractionBehavior
    {
        void Behave(Entity entitySource, Entity entityTarget, GlobalSimulationParameters simulationParams,
            Random random, InteractionService interactionService);
    }
}