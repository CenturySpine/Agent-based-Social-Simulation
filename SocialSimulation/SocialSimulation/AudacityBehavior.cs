using System;

namespace SocialSimulation
{
    public class AudacityBehavior : IEntityBehavior
    {
        public void Behave(Entity entity, GlobalSimulationParameters simulationParams, Random random)
        {
            var audacityInfluence = random.NextDouble() < entity.Audacity;

            if (audacityInfluence)
            {
                entity.Direction = (StartDirection)random.Next(0, 4);
            }
            else
            {
                entity.Continuation++;
            }
        }
    }
}