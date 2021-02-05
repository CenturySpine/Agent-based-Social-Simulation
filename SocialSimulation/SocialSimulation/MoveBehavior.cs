using System;
using System.Numerics;

namespace SocialSimulation
{
    public class MoveBehavior : IEntityBehavior
    {
        private readonly Random _rnd = new Random(DateTime.Now.Millisecond);
        private readonly Logger _logger;
        private readonly InteractionService _interactionService;

        public MoveBehavior(Logger logger, InteractionService interactionService)
        {
            _logger = logger;
            _interactionService = interactionService;
        }

        public void Behave(Entity entity, GlobalSimulationParameters simulationParams, Random random)
        {
            if (entity.State == EntityState.Moving)
            {
                Vector2 start = new Vector2(entity.Position.X, entity.Position.Y);

                IDirectionInitiator directionInitiator = null;
                if (entity.IsMovingTowardGoal == MovementType.Stopped)
                {

                    _logger.Log("Defining basic - straight line - goal... ");

                    directionInitiator = new StraightNavigationBehavior(_logger);

                }
                else if (entity.IsMovingTowardGoal == MovementType.TowardGoal && entity.Goal != null)
                {
                    _logger.Log("Defining goal... ");
                    directionInitiator = new GoalNavigationBehavior(_logger);
                }

                if (directionInitiator != null)
                {
                    Vector2 end = directionInitiator.InitiateDirectionGoal(entity, simulationParams);

                    float distance = Vector2.Distance(start, end);
                    Vector2 direction = Vector2.Normalize(end - start);

                    entity.Position = start;
                    entity.CurrentMoveData = new MoveData { Dir = direction, distance = distance, start = start, end = end };

                    entity.IsMovingTowardGoal = MovementType.StraightLine;

                    if (float.IsNaN(entity.CurrentMoveData.Dir.X) || float.IsNaN(entity.CurrentMoveData.Dir.Y))
                    {
                        //do something
                    }
                }

                Move(entity, simulationParams);
            }
        }

        private class DirectionSwitchBounce : IDirectionSwitch
        {
            void IDirectionSwitch.Switch(Entity entity, Random rnd)
            {
                //basic bounce behavior : at the end of the path, just turn around and move in the opposite direction
                switch (entity.Direction)
                {
                    case StartDirection.Left:
                        entity.Direction = StartDirection.Right;
                        break;

                    case StartDirection.Top:
                        entity.Direction = StartDirection.Bottom;
                        break;

                    case StartDirection.Right:
                        entity.Direction = StartDirection.Left;
                        break;

                    case StartDirection.Bottom:
                        entity.Direction = StartDirection.Top;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private class DirectionSwitchRandom : IDirectionSwitch
        {
            void IDirectionSwitch.Switch(Entity entity, Random rnd)
            {
                entity.Direction = (StartDirection)rnd.Next(0, 4);
            }
        }

        private void Move(Entity entity, GlobalSimulationParameters globalSimulationParameters)
        {
            MoveData data = entity.CurrentMoveData;

            if (data == null)
                return;

            entity.Position = entity.Position
                              + data.Dir //direction
                              * (float)entity.Speed //entity speed
                              * (1000f / 60f); //refresh frequency

            if (float.IsNaN(entity.Position.X) || float.IsNaN(entity.Position.Y))
            {
                entity.Position = new Vector2(0.0f, 0.0f);
            }

            UpdatePersonalSpace(entity, globalSimulationParameters);

            entity.Bound = new BoundBox
            {
                x = (float)entity.PersonalSpaceOrigin.X,
                y = (float)(entity.PersonalSpaceOrigin.Y),
                width = entity.PersonalSpaceSize * 2,
                height = entity.PersonalSpaceSize * 2,
            };

            if (Vector2.Distance(data.start, entity.Position) >= data.distance)
            {
                entity.Position = data.end;

                IDirectionSwitch switchBehavior;
                if (entity.Goal != null)
                {
                    _logger.Log("Goal reached!!!");

                    entity.Goal = null;
                    switchBehavior = new DirectionSwitchRandom();
                }
                else
                {
                    _logger.Log("End of the line");
                    switchBehavior = new DirectionSwitchBounce();
                }

                //trigger goal definition on next loop
                entity.IsMovingTowardGoal = MovementType.Stopped;
                switchBehavior.Switch(entity, _rnd);
            }
        }

        public static void UpdatePersonalSpace(Entity entity, GlobalSimulationParameters para)
        {
            var topleft = new Vector2(entity.Position.X + para.entitySize / 2 - (float)entity.PersonalSpaceSize, entity.Position.Y + para.entitySize / 2 - (float)entity.PersonalSpaceSize);

            entity.PersonalSpaceOrigin = topleft;
        }
    }
}