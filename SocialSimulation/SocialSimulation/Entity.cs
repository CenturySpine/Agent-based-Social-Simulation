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
            set { _position = value;OnPropertyChanged(); }
        }

        public StartDirection StartDirection { get; set; }


    }

    public enum StartDirection
    {
        Left,
        Top,
        Right,
        Bottom
    }
}