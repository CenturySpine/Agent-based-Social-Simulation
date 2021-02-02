using System.Numerics;

namespace SocialSimulation
{
    public class Goal : NotifierBase
    {
        private Vector2 _goalPosition;

        public Vector2 GoalPosition
        {
            get => _goalPosition;
            set { _goalPosition = value; OnPropertyChanged(); }
        }
    }
}