using System.Numerics;

namespace SocialSimulation
{
    public class BoundBox
    {
        public Vector2 min { get; set; }
        public Vector2 max { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public double width { get; set; }
        public double height { get; set; }
    }
}