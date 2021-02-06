using SocialSimulation.Collisions;
using SocialSimulation.Core;
using System.Numerics;

namespace SocialSimulation.Entity
{
    public class PersonalSpace : NotifierBase
    {
        private Vector2 _origin;

        public Vector2 Origin
        {
            get => _origin;
            set { _origin = value; OnPropertyChanged(); }
        }

        public double Size
        {
            get => _size;
            set { _size = value; OnPropertyChanged(); }
        }

        private double _size;

        public BoundBox Bound { get; set; }
    }
}