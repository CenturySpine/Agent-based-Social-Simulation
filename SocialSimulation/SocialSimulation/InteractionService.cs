using System;
using System.Collections.Generic;
using SocialSimulation.Entity;

namespace SocialSimulation
{
    public class InteractionService
    {
        private readonly Logger _logger;
        private readonly object _interLock = new object();
        public InteractionService(Logger logger)
        {
            _logger = logger;
        }
        private readonly Dictionary<Guid, Interaction> _interactions = new Dictionary<Guid, Interaction>();

        public void Interact(Entity.Entity entitySource, Entity.Entity entityTarget, Random random)
        {
            lock (_interLock)
            {
                //do not interact if both already interacting
                if (entitySource.State == EntityState.Talking && entityTarget.State == EntityState.Talking)
                    return;

                //do not interact if social latency is not elapsed
                if (entitySource.Social.CurrentSocialLatency < entitySource.Social.SocialLatencyThreshold || entityTarget.Social.CurrentSocialLatency < entityTarget.Social.SocialLatencyThreshold)
                    return;

                float interactProbability = entitySource.Social.NeedForSociability * entityTarget.Social.Charisma;
                float interactionIntensity = (float)(interactProbability - random.NextDouble());
                bool willInteract = interactionIntensity > 0;
                _logger.Log($"Entity {entitySource.Id} & Entity {entityTarget.Id} will interact = {willInteract}");
                if (willInteract)
                {
                    _logger.Log($"Entity {entitySource.Id} & Entity {entityTarget.Id} interact intensity = {interactionIntensity}");
                    entitySource.State = entityTarget.State = EntityState.Talking;

                    entitySource.RegisterCurrentMovement();
                    entityTarget.RegisterCurrentMovement();
                    entitySource.MovementType = entityTarget.MovementType = MovementType.Stopped;
                    Interaction i = new Interaction(entitySource, entityTarget, interactionIntensity);
                    _interactions.Add(i.InteractionId, i);
                    i.InteractionEnded += EndInteraction;
                    _logger.Log($"Entity {entitySource.Id} & Entity {entityTarget.Id} are interacting");
                }
            }
        }

        private void EndInteraction(Interaction i)
        {

            //reset state
            i.Entity1.State = i.Entity2.State = EntityState.Moving;

            //reset social latency : social exhaustion
            i.Entity1.Social.CurrentSocialLatency = 0;
            i.Entity2.Social.CurrentSocialLatency = 0;

            //resume movement 
            i.Entity1.ResumeMovement();
            i.Entity2.ResumeMovement();

            //clear completed interaction
            _interactions.Remove(i.InteractionId);

            _logger.Log($"Entity {i.Entity1.Id} & Entity {i.Entity2.Id} stopped interacting");
        }

        public void UpdateSocialLatency(Entity.Entity entity)
        {

            //social latency recovers while threshold is not reached and if entity is not interacting
            if (entity.State != EntityState.Talking &&
                entity.Social.CurrentSocialLatency < entity.Social.SocialLatencyThreshold)
            {
                entity.Social.CurrentSocialLatency += 
                                                      entity.Social.SocialLatencyRecoveryRate *
                                                      SimLoopData.Elapsed;

                //log end of social latency recovery
                if (entity.Social.CurrentSocialLatency >= entity.Social.SocialLatencyThreshold)
                    _logger.Log($"entity {entity.Id} is now ready for interactions !");
            }
        }
    }
}