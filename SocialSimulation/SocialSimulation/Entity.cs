using System.Windows;

namespace SocialSimulation
{
    public class Entity : NotifierBase
    {
        private Point _position;
        public int Id { get; set; }
        public int Initiative { get; set; }

        public Point Position
        {
            get => _position;
            set { _position = value; OnPropertyChanged(); }
        }

        public StartDirection Direction { get; set; }

        public double Speed { get; set; }

        public double Audacity { get; set; }

        public double Determination { get; set; }
        public int Continuation { get; set; }
    }
}