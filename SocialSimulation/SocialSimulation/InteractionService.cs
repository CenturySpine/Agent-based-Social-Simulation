using System;
using System.Collections.Generic;

namespace SocialSimulation
{
    public class InteractionService
    {
        private readonly GlobalSimulationParameters _simulationParams;
        private readonly Logger _logger;
        private object _interLock = new object();
        public InteractionService(GlobalSimulationParameters simulationParams, Logger logger)
        {
            _simulationParams = simulationParams;
            _logger = logger;
        }
        private readonly Dictionary<Guid, Interaction> _interactions = new Dictionary<Guid, Interaction>();

        public void Interact(Entity entitySource, Entity entityTarget, Random random)
        {
            lock (_interLock)
            {
                //do not interact if both already interacting
                if (entitySource.State == EntityState.Talking && entityTarget.State == EntityState.Talking)
                    return;
                
                float interactProbability = entitySource.NeedForSociability * entityTarget.Charisma;
                float interactionIntensity = (float)(interactProbability - random.NextDouble());
                bool willInteract = interactionIntensity > 0;
                _logger.Log($"Entity {entitySource.Id} & Entity {entityTarget.Id} will interact = {willInteract}");
                if (willInteract)
                {
                    _logger.Log($"Entity {entitySource.Id} & Entity {entityTarget.Id} interact intensity = {interactionIntensity}");
                    entitySource.State = entityTarget.State = EntityState.Talking;
                    entitySource.IsMovingTowardGoal = entityTarget.IsMovingTowardGoal = MovementType.Stopped;
                    Interaction i = new Interaction(entitySource, entityTarget, interactionIntensity);
                    _interactions.Add(i.InteractionId, i);
                    i.InteractionEnded += EndInteraction;
                    _logger.Log($"Entity {entitySource.Id} & Entity {entityTarget.Id} are interacting");
                }
            }
        }

        private void EndInteraction(Interaction i)
        {
            i.Entity1.State = i.Entity2.State = EntityState.Moving;
            i.Entity1.IsMovingTowardGoal = i.Entity2.IsMovingTowardGoal = MovementType.Stopped;
            _interactions.Remove(i.InteractionId);
            _logger.Log($"Entity {i.Entity1.Id} & Entity {i.Entity2.Id} stopped interacting");
        }
    }
}