using System;
using System.Windows.Media;
using SocialSimulation.Game;

namespace SocialSimulation.GameLoop
{
    internal class CompositionTargetLoop : IGameLoop
    {
        private IGame _game;
        private DateTime _currentTime;

        public void Start(IGame game)
        {
            _game = game;
            _currentTime = DateTime.Now;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, System.EventArgs e)
        {
            var ellapsed = DateTime.Now.Subtract(_currentTime);
            _game.Input();
            _game.Update((float)ellapsed.TotalMilliseconds);
            _game.Render(SimLoopData.DesiredElapsed);
            _currentTime = DateTime.Now;

        }

        public void Stop(IGame game)
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }
    }
}