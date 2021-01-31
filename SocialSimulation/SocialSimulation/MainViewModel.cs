using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SocialSimulation
{
    public class MainViewModel : NotifierBase
    {
        private int _surfaceWidth;
        private int _unitsNumber;
        private int _surfaceHeight;
        private Random _rnd;
        private List<Entity> _entities;
        private Random xRnd;
        private Random yRnd;
        private Timer _moveTimer;

        public List<Entity> Entities
        {
            get => _entities;
            private set { _entities = value; OnPropertyChanged(); }
        }

        public double Speed
        {
            get => _speed;
            set
            {
                _speed = value; OnPropertyChanged();
                ChangeSpeed();
            }
        }

        private int _baseFrameRate = 16;
        private void ChangeSpeed()
        {
            if (_moveTimer != null)
            {
                _moveTimer.Change(0, (int)(_baseFrameRate / _speed));
            }
        }

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


        private void ExecuteStopMove(object o)
        {
            _moveTimer?.Dispose();
            _moveTimer = null;
            _stoped = true;
        }

        public RelayCommand StopMoveCommand { get; set; }

        private void ExecuteStartMove(object o)
        {
            _stoped = false;
            _moveTimer = new Timer(OnMove, null, 1000, 16);
        }

        private void OnMove(object state)
        {
            if(_stoped)
                return;

            foreach (var entity in Entities)
            {
                switch (entity.StartDirection)
                {
                    case StartDirection.Left:
                        entity.Position = new Point(entity.Position.X - 1, entity.Position.Y);
                        if (entity.Position.X <= 0)
                        {
                            entity.StartDirection = StartDirection.Right;
                        }
                        break;
                    case StartDirection.Top:
                        entity.Position = new Point(entity.Position.X, entity.Position.Y - 1);
                        if (entity.Position.Y <= 0)
                        {
                            entity.StartDirection = StartDirection.Bottom;
                        }
                        break;
                    case StartDirection.Right:
                        entity.Position = new Point(entity.Position.X + 1, entity.Position.Y);
                        if (entity.Position.X >= SurfaceWidth - entitySize)
                        {
                            entity.StartDirection = StartDirection.Left;
                        }
                        break;
                    case StartDirection.Bottom:
                        entity.Position = new Point(entity.Position.X, entity.Position.Y + 1);
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
        int entitySize = 10;
        private double _speed;
        private bool _stoped;
        public RelayCommand StartMoveCommand { get; set; }

        private void GenerateRng()
        {
            _rnd = new Random(DateTime.Now.Millisecond);
            xRnd = new Random(DateTime.Now.Second);
            yRnd = new Random(DateTime.Now.Millisecond + DateTime.Now.Day);
        }

        private async void ExecuteGenerate(object o)
        {
            await Task.Run(() =>
            {
                Entities.Clear();
                var ets = new List<Entity>();
                for (int i = 0; i < UnitsNumber; i++)
                {

                    var e = new Entity() { Id = i + 1 };

                    var x = xRnd.Next(entitySize, SurfaceWidth - entitySize) - entitySize / 2;

                    var y = yRnd.Next(entitySize, SurfaceHeight - entitySize) - entitySize / 2;

                    e.StartDirection = (StartDirection)_rnd.Next(0, 4);
                    e.Position = new Point(x, y);
                    ets.Add(e);


                }

                Entities = ets;
            });


        }

        public RelayCommand GenerateCommand { get; set; }

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

        public int SurfaceHeight
        {
            get => _surfaceHeight;

            set { _surfaceHeight = value; OnPropertyChanged(); }
        }

    }
}