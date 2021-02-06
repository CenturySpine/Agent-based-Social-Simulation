using System.Numerics;

namespace SocialSimulation.Movement
{
    public class MoveData
    {
        public Vector2 Dir { get; set; }
        public float distance { get; set; }
        public Vector2 end { get; set; }
        public Vector2 start { get; set; }
    }
}