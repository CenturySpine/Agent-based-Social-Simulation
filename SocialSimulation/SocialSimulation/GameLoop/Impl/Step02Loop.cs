using System;
using System.Threading.Tasks;
using SocialSimulation.Game;

namespace SocialSimulation.GameLoop.Impl
{
    internal class Step02Loop : ICustomLoopBehavior
    {
        private bool _running;

        public void Start(IGame game)
        {
            TimeSpan lastTime = TimeSpan.FromTicks(DateTime.Now.Ticks);
            _running = true;
            Task.Run(() =>
            {
                while (_running)
                {
                    TimeSpan current = TimeSpan.FromTicks(DateTime.Now.Ticks);
                    TimeSpan elapsed = current - lastTime;
                    game.Input();
                    game.Update(elapsed.Milliseconds);
                    game.Render(elapsed.Milliseconds);

                    lastTime = current;
                }
            });
        }

        public void Stop(IGame game)
        {
            _running = false;
            game.Unload();
        }
    }
}