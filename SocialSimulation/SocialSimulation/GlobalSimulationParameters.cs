namespace SocialSimulation
{
    public class GlobalSimulationParameters : NotifierBase
    {
        public GlobalSimulationParameters()
        {
            SurfaceHeight = 300;
            SurfaceWidth = 300;
            UnitsNumber = 1;
            EntitySize = 10;
            Speed = 0.1;
            MinSpeed = 0.0;
            MaxSpeed = 1.0;
            Audacity = 0.01;
            PersonalSpace = 20;
        }

        public double MinSpeed { get; set; }

        public double MaxSpeed { get; set; }

        public double PersonalSpace
        {
            get => _personalSpace;
            set
            {
                _personalSpace = value; OnPropertyChanged();
            }
        }

        private double _audacity;

        public double Audacity
        {
            get => _audacity;
            set
            {
                _audacity = value; OnPropertyChanged();
                //ChangeAudacity();
            }
        }

        public double Speed
        {
            get => _speed;
            set
            {
                _speed = value; OnPropertyChanged();
                //ChangeSpeed();
            }
        }

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

        public int EntitySize { get; set; }

        private int _surfaceHeight;
        private int _surfaceWidth;
        private int _unitsNumber;
        private double _speed;
        private double _personalSpace;
    }
}