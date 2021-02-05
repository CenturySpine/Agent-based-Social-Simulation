namespace SocialSimulation.Entity
{
    public class Movement : NotifierBase
    {
        public StartDirection Direction { get; set; }

        public double Speed { get; set; }

        public MoveData CurrentMoveData { get; set; }
    }
}