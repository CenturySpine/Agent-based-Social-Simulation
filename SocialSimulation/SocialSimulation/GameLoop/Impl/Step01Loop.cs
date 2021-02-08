using System;
using System.Threading;
using System.Threading.Tasks;
using SocialSimulation.Game;

namespace SocialSimulation.GameLoop.Impl
{
    internal class Step01Loop : ICustomLoopBehavior
    {
        private TimeSpan MS_PER_FRAME = new TimeSpan(0, 0, 0, 0, (int)SimLoopData.DesiredElapsed);
        private bool _running;

        public void Start(IGame game)
        {
            _running = true;
            Task.Run(() =>
            {
                while (_running)
                {
                    TimeSpan start = TimeSpan.FromTicks(DateTime.Now.Ticks);
                    game.Input();
                    game.Update(1);
                    game.Render(1);

                    int sleepTime = Math.Max(start.Add(MS_PER_FRAME.Subtract(TimeSpan.FromTicks(DateTime.Now.Ticks))).Milliseconds, 0);
                    TimeSpan sleep = TimeSpan.FromMilliseconds(sleepTime);
                    Thread.Sleep(sleep);
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