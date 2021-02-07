using SocialSimulation.SimulationParameters;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SocialSimulation.Core;

namespace SocialSimulation.Game
{
    public interface IGameSurface
    {
        Panel Surface { get; set; }

        void Update();
    }

    public class CanvasGameSurface : IGameSurface
    {
        private readonly GlobalSimulationParameters _simParams;

        public CanvasGameSurface(GlobalSimulationParameters simParams)
        {
            _simParams = simParams;
            Surface = new Canvas
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidColorBrush(Colors.LightGray)
            };
        }

        public Panel Surface { get; set; }

        public void Update()
        {
            UiRenderer.Render(() =>
            {
                Surface.Height = _simParams.SurfaceHeight;
                Surface.Width = _simParams.SurfaceWidth;
            });
        }
    }
}