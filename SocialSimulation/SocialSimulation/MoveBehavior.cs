using SocialSimulation.Entity;
using System;
using System.Linq;
using System.Numerics;

namespace SocialSimulation
{
    public class MoveBehavior : IEntityBehavior
    {
        private readonly Random _rnd = new Random(DateTime.Now.Millisecond);
        private readonly Logger _logger;

        public MoveBehavior(Logger logger)
        {
            _logger = logger;
        }

        public void Behave(Entity.Entity entity, GlobalSimulationParameters simulationParams, Random random)
        {
            if (entity.State == EntityState.Moving)
            {
                Vector2 start = new Vector2(entity.Position.X, entity.Position.Y);

                IDirectionInitiator directionInitiator = null;
                if (entity.MovementType == MovementType.Stopped)
                {
                    _logger.Log("Defining basic - straight line - goal... ");

                    directionInitiator = new StraightNavigationBehavior(_logger);
                }
                else if (entity.MovementType == MovementType.TowardGoal && entity.CurrentGoal != null)
                {
                    _logger.Log("Defining goal... ");
                    directionInitiator = new GoalNavigationBehavior(_logger);
                }

                if (directionInitiator != null)
                {
                    Vector2 end = directionInitiator.InitiateDirectionGoal(entity, simulationParams);
                    if (end != Vector2.Zero)
                    {
                        float distance = Vector2.Distance(start, end);
                        Vector2 direction = Vector2.Normalize(end - start);

                        entity.Position = start;
                        entity.Movement.CurrentMoveData = new MoveData { Dir = direction, distance = distance, start = start, end = end };

                        entity.MovementType = MovementType.StraightLine;

                        if (float.IsNaN(entity.Movement.CurrentMoveData.Dir.X) || float.IsNaN(entity.Movement.CurrentMoveData.Dir.Y))
                        {
                            //do something
                        }
                    }
                }

                Move(entity);
            }
        }

        private class DirectionSwitchBounce : IDirectionSwitch
        {
            void IDirectionSwitch.Switch(Entity.Entity entity, Random rnd)
            {
                //basic bounce behavior : at the end of the path, just turn around and move in the opposite direction
                switch (entity.Movement.Direction)
                {
                    case StartDirection.Left:
                        entity.Movement.Direction = StartDirection.Right;
                        break;

                    case StartDirection.Top:
                        entity.Movement.Direction = StartDirection.Bottom;
                        break;

                    case StartDirection.Right:
                        entity.Movement.Direction = StartDirection.Left;
                        break;

                    case StartDirection.Bottom:
                        entity.Movement.Direction = StartDirection.Top;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private class DirectionSwitchRandom : IDirectionSwitch
        {
            void IDirectionSwitch.Switch(Entity.Entity entity, Random rnd)
            {
                entity.Movement.Direction = (StartDirection)rnd.Next(0, 4);
            }
        }

        private void Move(Entity.Entity entity)
        {
            MoveData data = entity.Movement.CurrentMoveData;

            entity.Position += data.Dir //direction
                               * (float)entity.Movement.Speed //entity speed
                               * SimLoopData.Elapsed; //refresh frequency

            if (float.IsNaN(entity.Position.X) || float.IsNaN(entity.Position.Y))
            {
                entity.Position = new Vector2(0.0f, 0.0f);
            }

            UpdatePersonalSpace(entity);

            UpdatePersonalSpaceBoundBox(entity);

            if (Vector2.Distance(data.start, entity.Position) >= data.distance)
            {
                entity.Position = data.end;

                IDirectionSwitch switchBehavior = null;
                if (entity.CurrentGoal != null)
                {
                    _logger.Log("Goal reached!!!");

                    entity.Goals.Remove(entity.CurrentGoal);
                    entity.CurrentGoal = null;
                    
                    if (entity.Goals.Any())
                    {
                        entity.CurrentGoal = entity.Goals.First();
                        entity.MovementType = MovementType.TowardGoal;
                    }
                    else
                    {
                        switchBehavior = new DirectionSwitchRandom();
                        entity.MovementType = MovementType.Stopped;
                    }
                    
                }
                else
                {
                    _logger.Log("End of the line");
                    switchBehavior = new DirectionSwitchBounce();
                    entity.MovementType = MovementType.Stopped;
                }

                //trigger goal definition on next loop
                
                switchBehavior?.Switch(entity, _rnd);
            }
        }

        private static void UpdatePersonalSpaceBoundBox(Entity.Entity entity)
        {
            entity.PersonalSpace.Bound = new BoundBox
            {
                X = entity.PersonalSpace.Origin.X,
                Y = entity.PersonalSpace.Origin.Y,
                Width = entity.PersonalSpace.Size * 2,
                Height = entity.PersonalSpace.Size * 2,
            };
        }

        public static void UpdatePersonalSpace(Entity.Entity entity)
        {
            var topleft = new Vector2(entity.Position.X + entity.SelfSize / 2 - (float)entity.PersonalSpace.Size, entity.Position.Y + entity.SelfSize / 2 - (float)entity.PersonalSpace.Size);

            entity.PersonalSpace.Origin = topleft;
        }
    }
}