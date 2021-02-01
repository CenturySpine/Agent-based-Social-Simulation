using System.Windows;

namespace SocialSimulation
{
    public class Entity : NotifierBase
    {
        public Entity()
        {
            Goal = new Goal();
        }
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
        public double Continuation { get; set; }

        public Goal Goal { get; set; }
    }

    public class Goal : NotifierBase
    {
        private Point _goalPosition;

        public Point GoalPosition
        {
            get => _goalPosition;
            set { _goalPosition = value; OnPropertyChanged(); }
        }
    }
}