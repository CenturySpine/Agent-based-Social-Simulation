using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SocialSimulation
{
    public class MainViewModel : NotifierBase
    {
        private List<Entity> _entities;
        private Timer _moveTimer;
        private Random _rnd;
        private double _speed;
        private bool _stopped;
        private int _surfaceHeight;
        private int _surfaceWidth;
        private int _unitsNumber;
        private Random _xRnd;
        private Random _yRnd;
        private int entitySize = 10;

        public MainViewModel()
        {
            SurfaceHeight = 100;
            SurfaceWidth = 100;
            UnitsNumber = 10;
            GenerateCommand = new RelayCommand(ExecuteGenerate);
            StartMoveCommand = new RelayCommand(ExecuteStartMove);
            StopMoveCommand = new RelayCommand(ExecuteStopMove);
            Entities = new List<Entity>();
            Speed = 1;
            GenerateRng();
        }

        public List<Entity> Entities
        {
            get => _entities;
            private set { _entities = value; OnPropertyChanged(); }
        }

        public RelayCommand GenerateCommand { get; set; }

        public double Speed
        {
            get => _speed;
            set
            {
                _speed = value; OnPropertyChanged();
                ChangeSpeed();
            }
        }

        public RelayCommand StartMoveCommand { get; set; }

        public RelayCommand StopMoveCommand { get; set; }

        public int SurfaceHeight
        {
            get => _surfaceHeight;

            set { _surfaceHeight = value; OnPropertyChanged(); }
        }

        public int SurfaceWidth
        {
            get => _surfaceWidth;
            set { _surfaceWidth = value; OnPropertyChanged(); }
        }

        public int UnitsNumber
        {
            get => _unitsNumber;
            set { _unitsNumber = value; OnPropertyChanged(); }
        }

        private void ChangeSpeed()
        {
            foreach (var entity in Entities)
            {
                entity.Speed = _speed;
            }
        }

        private async void ExecuteGenerate(object o)
        {
            await Task.Run(() =>
            {
                Entities.Clear();
                var ets = new List<Entity>();
                for (int i = 0; i < UnitsNumber; i++)
                {
                    var e = new Entity() { Id = i + 1, Speed = Speed };

                    var x = _xRnd.Next(entitySize, SurfaceWidth - entitySize) - entitySize / 2;

                    var y = _yRnd.Next(entitySize, SurfaceHeight - entitySize) - entitySize / 2;

                    e.StartDirection = (StartDirection)_rnd.Next(0, 4);
                    e.Position = new Point(x, y);
                    ets.Add(e);
                }

                Entities = ets;
            });
        }

        private void ExecuteStartMove(object o)
        {
            _stopped = false;
            _moveTimer = new Timer(OnMove, null, 1000, 16);
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

        private void OnMove(object state)
        {
            if (_stopped)
                return;

            foreach (var entity in Entities)
            {
                switch (entity.StartDirection)
                {
                    case StartDirection.Left:
                        entity.Position = new Point(entity.Position.X - entity.Speed, entity.Position.Y);
                        if (entity.Position.X <= 0)
                        {
                            entity.StartDirection = StartDirection.Right;
                        }
                        break;

                    case StartDirection.Top:
                        entity.Position = new Point(entity.Position.X, entity.Position.Y - entity.Speed);
                        if (entity.Position.Y <= 0)
                        {
                            entity.StartDirection = StartDirection.Bottom;
                        }
                        break;

                    case StartDirection.Right:
                        entity.Position = new Point(entity.Position.X + entity.Speed, entity.Position.Y);
                        if (entity.Position.X >= SurfaceWidth - entitySize)
                        {
                            entity.StartDirection = StartDirection.Left;
                        }
                        break;

                    case StartDirection.Bottom:
                        entity.Position = new Point(entity.Position.X, entity.Position.Y + entity.Speed);
                        if (entity.Position.Y >= SurfaceHeight - entitySize)
                        {
                            entity.StartDirection = StartDirection.Top;
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}