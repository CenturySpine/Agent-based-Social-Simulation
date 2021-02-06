using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace SocialSimulation.Entity
{
    public class Entity : NotifierBase
    {
        public Entity()
        {
            Goals = new List<Goal>();
            Movement = new Movement();
            PersonalSpace = new PersonalSpace();
            Social = new Social();
        }

        public Social Social { get; }
        public Movement Movement { get; }
        public PersonalSpace PersonalSpace { get; }
        private Vector2 _position;

        private List<Entity> _collidingEntities;
        private bool _isColliding;

        private EntityState _state;
        private MovementType _storedMoveType;
        public int Id { get; set; }

        public Vector2 Position
        {
            get => _position;
            set { _position = value; OnPropertyChanged(); }
        }

        public EntityState State
        {
            get => _state;
            set { _state = value; OnPropertyChanged(); }
        }

        public double Audacity { get; set; }

        public double Determination { get; set; }
        public double Continuation { get; set; }

        public List<Goal> Goals { get; set; }
        public MovementType MovementType { get; set; }
        public Goal CurrentGoal { get; set; }
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

        public int SelfSize { get; set; }

        public void RegisterCurrentMovement()
        {
            _storedMoveType = MovementType;
        }
        public void ResumeMovement()
        {
            MovementType= _storedMoveType;
        }
    }
}