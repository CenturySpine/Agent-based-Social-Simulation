using SocialSimulation.Collisions;
using SocialSimulation.Core;
using SocialSimulation.Entity;
using SocialSimulation.Environment;
using SocialSimulation.Interactions;
using SocialSimulation.Movement;
using SocialSimulation.SimulationParameters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SocialSimulation
{
    public class MainViewModel : NotifierBase
    {
        public GlobalSimulationParameters SimulationParams { get; }
        public EnvironmentService Environment { get; }
        private readonly MovementService _movement;
        private readonly Logger _logger;
        private readonly CollisionService _collisions;
        private readonly InteractionService _interactService;
        private List<Entity.Entity> _entities;
        private Timer _moveTimer;
        private Random _rnd;

        private bool _stopped;

        private Random _xRnd;
        private Random _yRnd;

        public MainViewModel(GlobalSimulationParameters simulationParams, MovementService movement, Logger logger, CollisionService collisions, InteractionService interactService, EnvironmentService environment)
        {
            SimulationParams = simulationParams;
            Environment = environment;
            _movement = movement;
            _logger = logger;
            _collisions = collisions;
            _interactService = interactService;
            SimulationParams.PropertyChanged += SimulationParamsOnPropertyChanged;

            GenerateCommand = new RelayCommand(ExecuteGenerate);
            StartMoveCommand = new RelayCommand(ExecuteStartMove);
            StopMoveCommand = new RelayCommand(ExecuteStopMove);

            Entities = new List<Entity.Entity>();
            Logs = new ObservableCollection<string>();

            _logger.RegisterListener(InternalLog);

            GenerateRng();
        }

        public ObservableCollection<string> Logs { get; set; }

        private void InternalLog(string obj)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Logs.Insert(0, obj);
            }));
        }

        private void SimulationParamsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GlobalSimulationParameters.Speed))
                ChangeSpeed();

            if (e.PropertyName == nameof(GlobalSimulationParameters.PersonalSpace))
                ChangeDetermination();

            if (e.PropertyName == nameof(GlobalSimulationParameters.Audacity))
                ChangeAudacity();
        }

        private void ChangeDetermination()
        {
            lock (_entitiesLock)
            {
                foreach (var entity in Entities)
                {
                    _logger.Log($"Changing {nameof(SimulationParams.PersonalSpace)} to {SimulationParams.PersonalSpace}");
                    entity.PersonalSpace.Size = SimulationParams.PersonalSpace;
                }
            }
        }

        public List<Entity.Entity> Entities
        {
            get => _entities;
            private set { _entities = value; OnPropertyChanged(); }
        }

        public RelayCommand GenerateCommand { get; }

        public RelayCommand StartMoveCommand { get; }

        public RelayCommand StopMoveCommand { get; }

        private void ChangeAudacity()
        {
            lock (_entitiesLock)
            {
                foreach (var entity in Entities)
                {
                    _logger.Log($"Changing {nameof(SimulationParams.Audacity)} to {SimulationParams.Audacity}");
                    entity.Audacity = SimulationParams.Audacity;
                }
            }
        }

        private void ChangeSpeed()
        {
            lock (_entitiesLock)
            {
                foreach (var entity in Entities)
                {
                    _logger.Log($"Changing {nameof(SimulationParams.Speed)} to {SimulationParams.Speed}");
                    entity.Movement.Speed = SimulationParams.Speed;
                }
            }
        }

        private async void ExecuteGenerate(object o)
        {
            await Task.Run(() =>
            {
                lock (_entitiesLock)
                {
                    Entities.Clear();
                    var ets = new List<Entity.Entity>();
                    for (int i = 0; i < SimulationParams.UnitsNumber; i++)
                    {
                        var e = new Entity.Entity { Id = i + 1, Audacity = SimulationParams.Audacity, SelfSize = SimulationParams.EntitySize };
                        e.Movement.Speed = SimulationParams.Speed;
                        var x = _xRnd.Next(e.SelfSize, SimulationParams.SurfaceWidth - e.SelfSize) - e.SelfSize / 2;

                        var y = _yRnd.Next(e.SelfSize, SimulationParams.SurfaceHeight - e.SelfSize) - e.SelfSize / 2;

                        e.Movement.Direction = (StartDirection)_rnd.Next(0, 4);
                        e.Position = new Vector2(x, y);

                        e.Social.Charisma = (float)_rnd.NextDouble();
                        e.Social.NeedForSociability = (float)_rnd.NextDouble();
                        e.Social.SocialLatencyThreshold = _rnd.Next(20, 80);
                        e.Social.CurrentSocialLatency = e.Social.SocialLatencyThreshold;
                        e.Social.SocialLatencyRecoveryRate = 0.005f;

                        e.PersonalSpace.Size = SimulationParams.PersonalSpace;
                        MoveBehavior.UpdatePersonalSpace(e);

                        ets.Add(e);
                    }

                    Entities = ets;
                }
                _logger.Log($"Generation completed");
            });
        }

        private void ExecuteStartMove(object o)
        {
            _logger.Log($"Starting movement");
            _stopped = false;
            Entities.ForEach(e => e.State = EntityState.Moving);
            _moveTimer = new Timer(OnUpdateEntities, null, 1000, (int)SimLoopData.Elapsed);
        }

        private void ExecuteStopMove(object o)
        {
            _logger.Log($"Stopping movement");
            _moveTimer?.Dispose();
            _moveTimer = null;
            _stopped = true;
        }

        private void GenerateRng()
        {
            _rnd = new Random(DateTime.Now.Millisecond);
            _xRnd = new Random(DateTime.Now.Second);
            _yRnd = new Random(DateTime.Now.Millisecond + DateTime.Now.Day);
        }

        private readonly object _entitiesLock = new object();

        private void OnUpdateEntities(object state)
        {
            if (_stopped)
                return;

            lock (_entitiesLock)
            {
                var copy = Entities.ToList();
                Parallel.ForEach(Entities, e =>
                {
                    _movement.Update(e);
                    _interactService.UpdateSocialLatency(e);
                });

                foreach (var entity in Entities)
                {
                    _collisions.ComputeCollision(entity, copy);
                }

                foreach (var entity in Entities.Where(e => e.CollidingEntities.Any()))
                {
                    entity.CollidingEntities.ForEach((e) => _interactService.Interact(entity, e, _rnd));
                }
                Environment.UpdateTime(SimLoopData.Elapsed);
            }
        }

        public void SetGoal(Point getPosition)
        {
            lock (_entitiesLock)
            {
                foreach (var entity in Entities)
                {
                    var addedGoal = new Goal { GoalPosition = new Vector2((float)getPosition.X, (float)getPosition.Y) };
                    entity.Goals.Add(addedGoal);
                    if (entity.CurrentGoal == null)
                    {
                        entity.CurrentGoal = addedGoal;
                    }
                    entity.MovementType = MovementType.TowardGoal;
                }
            }
        }
    }
}