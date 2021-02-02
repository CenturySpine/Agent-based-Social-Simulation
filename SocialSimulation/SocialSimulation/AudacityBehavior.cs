using System;
using System.Collections.Generic;

namespace SocialSimulation
{
    public class AudacityBehavior : IEntityBehavior
    {
        private readonly Logger _logger;

        public AudacityBehavior(Logger logger)
        {
            _logger = logger;
        }
        public void Behave(Entity entity, GlobalSimulationParameters simulationParams, Random random,
            Dictionary<Entity, MoveData> goalTrack)
        {
            var audacityInfluence = random.NextDouble() < entity.Audacity /*&& (entity.Continuation > entity.PersonalSpace)*/;

            if (audacityInfluence && entity.Goal == null)
            {
                var newDir = entity.Direction;
                while (newDir == entity.Direction)
                {
                    newDir = (StartDirection)random.Next(0, 4);
                }
                //_logger.Log($"Entity {entity.Id} changed direction from {entity.Direction} to {newDir}");
                entity.Direction = newDir;
                //entity.Continuation = 0.0;
                entity.IsMovingTowardGoal = MovementType.Stopped;
            }
            else
            {
                //entity.Continuation += 0.05;
            }
        }
    }
}