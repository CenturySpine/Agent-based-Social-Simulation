using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace SocialSimulation
{
    public class Entity : NotifierBase
    {
        public Entity()
        {
            Goal = new Goal();
        }

        private Vector2 _position;
        private Vector2 _personalSpaceOrigin;
        private List<Entity> _collidingEntities;
        private bool _isColliding;
        private double _personalSpaceSize;
        public int Id { get; set; }
        public int Initiative { get; set; }

        public Vector2 Position
        {
            get => _position;
            set { _position = value; OnPropertyChanged(); }
        }

        public Vector2 PersonalSpaceOrigin
        {
            get => _personalSpaceOrigin;
            set { _personalSpaceOrigin = value; OnPropertyChanged(); }
        }

        public StartDirection Direction { get; set; }

        public double Speed { get; set; }

        public double Audacity { get; set; }

        public double Determination { get; set; }
        public double Continuation { get; set; }

        public Goal Goal { get; set; }
        public MovementType IsMovingTowardGoal { get; set; }

        public double PersonalSpaceSize
        {
            get => _personalSpaceSize;
            set { _personalSpaceSize = value; OnPropertyChanged(); }
        }

        public MoveData CurrentMoveData { get; set; }
        public BoundBox Bound { get; set; }

        public bool IsColliding
        {
            get => _isColliding;
            set { _isColliding = value; OnPropertyChanged(); }
        }

        public List<Entity> CollidingEntities
        {
            get => _collidingEntities;
            set
            {
                _collidingEntities = value;
                IsColliding = value.Any();
            }
        }
    }
}