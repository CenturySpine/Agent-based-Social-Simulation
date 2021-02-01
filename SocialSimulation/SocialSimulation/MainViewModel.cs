using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SocialSimulation
{
    public class MainViewModel : NotifierBase
    {
        public GlobalSimulationParameters SimulationParams { get; }
        private readonly MovementService _movement;
        private List<Entity> _entities;
        private Timer _moveTimer;
        private Random _rnd;
        
        private bool _stopped;

        private Random _xRnd;
        private Random _yRnd;

        public MainViewModel(GlobalSimulationParameters simulationParams, MovementService movement)
        {
            SimulationParams = simulationParams;
            _movement = movement;
            SimulationParams.PropertyChanged+=SimulationParamsOnPropertyChanged;

            GenerateCommand = new RelayCommand(ExecuteGenerate);
            StartMoveCommand = new RelayCommand(ExecuteStartMove);
            StopMoveCommand = new RelayCommand(ExecuteStopMove);

            Entities = new List<Entity>();


            GenerateRng();
        }

        private void SimulationParamsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(GlobalSimulationParameters.Speed))
                ChangeSpeed();

            if (e.PropertyName == nameof(GlobalSimulationParameters.Determination))
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
                    entity.Determination = SimulationParams.Determination;
                }
            }
        }



        public List<Entity> Entities
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
                    entity.Speed = SimulationParams.Speed;
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
                    var ets = new List<Entity>();
                    for (int i = 0; i < SimulationParams.UnitsNumber; i++)
                    {
                        var e = new Entity() { Id = i + 1, Speed = SimulationParams.Speed, Audacity = SimulationParams.Audacity, Determination = SimulationParams.Determination };

                        var x = _xRnd.Next(SimulationParams.entitySize, SimulationParams.SurfaceWidth - SimulationParams.entitySize) - SimulationParams.entitySize / 2;

                        var y = _yRnd.Next(SimulationParams.entitySize, SimulationParams.SurfaceHeight - SimulationParams.entitySize) - SimulationParams.entitySize / 2;

                        e.Direction = (StartDirection)_rnd.Next(0, 4);
                        e.Position = new Point(x, y);
                        ets.Add(e);
                    }

                    Entities = ets;
                }
            });
        }

        private void ExecuteStartMove(object o)
        {
            _stopped = false;
            _moveTimer = new Timer(OnUpdateEntities, null, 1000, 16);
        }

        private void ExecuteStopMove(object o)
        {
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
                foreach (var entity in Entities)
                {

                    _movement.Update(entity);

                }
            }
        }
    }
}