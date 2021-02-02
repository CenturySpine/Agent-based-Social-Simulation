using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows;
using System.Windows.Shapes;

namespace SocialSimulation
{
    public interface IDirectionInitiator
    {
        Vector2 InitiateDirectionGoal(Entity entity, GlobalSimulationParameters parameters);
    }

    public class GoalNavigationBehavior : IDirectionInitiator
    {
        private readonly Logger _logger;

        public GoalNavigationBehavior(Logger logger)
        {
            _logger = logger;
        }

        public Vector2 InitiateDirectionGoal(Entity entity, GlobalSimulationParameters parameters)
        {
            var end = new Vector2(entity.Goal.GoalPosition.X, entity.Goal.GoalPosition.Y);
            _logger.Log($"Defined goal :{end}");
            return end;
        }
    }

    public class StraightNavigationBehavior : IDirectionInitiator
    {
        private readonly Logger _logger;

        public StraightNavigationBehavior(Logger logger)
        {
            _logger = logger;
        }

        public Vector2 InitiateDirectionGoal(Entity entity, GlobalSimulationParameters parameters)
        {
            Vector2 end;
            switch (entity.Direction)
            {
                case StartDirection.Left:
                    //move left = goal is the left border of surface, keeping top position constant
                    end = new Vector2(0, entity.Position.Y);
                    break;

                case StartDirection.Top:
                    //move top = goal is the top border of surface, keeping left position constant
                    end = new Vector2(entity.Position.X, 0);
                    break;

                case StartDirection.Right:
                    //move right = goal is the right border of surface, keeping top position constant
                    end = new Vector2(parameters.SurfaceWidth - parameters.entitySize, entity.Position.Y);
                    break;

                case StartDirection.Bottom:
                    //move bottom = goal is the bottom border of surface, keeping left position constant
                    end = new Vector2(entity.Position.X, parameters.SurfaceHeight - parameters.entitySize);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            _logger.Log($"Basic goal :{end}");
            _logger.Log($"Starting to move in straight line");
            return end;
        }
    }

    public class MoveBehavior : IEntityBehavior
    {
        private readonly Random _rnd = new Random(DateTime.Now.Millisecond);
        private readonly Logger _logger;

        public MoveBehavior(Logger logger)
        {
            _logger = logger;
        }

        public void Behave(Entity entity, GlobalSimulationParameters simulationParams, Random random, Dictionary<Entity, MoveData> goalTrack)
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
                goalTrack[entity] = new MoveData { Dir = direction, distance = distance, start = start, end = end };

                entity.IsMovingTowardGoal = MovementType.StraightLine;

                if (float.IsNaN(goalTrack[entity].Dir.X) || float.IsNaN(goalTrack[entity].Dir.Y))
                {
                    //do something
                }
            }

            Move(entity, goalTrack,simulationParams);
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

        private void Move(Entity entity, Dictionary<Entity, MoveData> goalTrack,
            GlobalSimulationParameters globalSimulationParameters)
        {
            MoveData data = goalTrack[entity];

            entity.Position = entity.Position
                              + data.Dir //direction
                              * (float)entity.Speed //entity speed
                              * (1000f / 60f); //refresh frequency

            if (float.IsNaN(entity.Position.X) || float.IsNaN(entity.Position.Y))
            {
                entity.Position = new Vector2(0.0f, 0.0f);
            }

            UpdatePersonalSpace(entity, globalSimulationParameters);

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
            var topleft = new Vector2(entity.Position.X+para.entitySize/2 - (float)entity.PersonalSpaceSize, entity.Position.Y + para.entitySize / 2 - (float)entity.PersonalSpaceSize);


            entity.PersonalSpaceOrigin = topleft;
        }
    }
}