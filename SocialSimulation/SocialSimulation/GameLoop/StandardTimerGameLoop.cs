using SocialSimulation.Game;
using System.Threading;

namespace SocialSimulation.GameLoop
{
    internal class StandardTimerGameLoop : IGameLoop
    {
        private Timer _moveTimer;
        private bool _running;

        public void Start(IGame game)
        {
            _running = true;
            _moveTimer = new Timer(OnUpdateEntities, game, 1000, (int)SimLoopData.Elapsed);
        }

        private void OnUpdateEntities(object state)
        {
            if (!_running) return;
            if (state is IGame game)
            {
                game.Input();
                game.Update(SimLoopData.Elapsed);
                game.Render(SimLoopData.Elapsed);
            }
        }

        public void Stop(IGame game)
        {
            _moveTimer?.Dispose();
            _moveTimer = null;
            _running = false;
            game.Unload();
        }
    }
}