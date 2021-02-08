using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SocialSimulation.Game;

namespace SocialSimulation.GameLoop.Impl
{
    internal class Step03Loop : ICustomLoopBehavior
    {
        private bool _running;
        //private TimeSpan MS_PER_FRAME = new TimeSpan(0, 0, 0, 0, (int)SimLoopData.DesiredElapsed);
        private float MS_PER_UPDATE = SimLoopData.DesiredElapsed;
        private Stopwatch _sw;
        private double _lastUpdate;

        public void Start(IGame game)
        {
            _sw = new Stopwatch();
            _sw.Reset();
            _sw.Start();
            _lastUpdate = _sw.Elapsed.TotalMilliseconds;
            float lag = 0.0f;
            _running = true;
            Task.Run(() =>
            {



                while (_running)
                {
                    double current = _sw.Elapsed.TotalMilliseconds;
                    float elapsed = (float) (current - _lastUpdate);


                    _lastUpdate = current;

                    lag += (float)elapsed;
                    Console.WriteLine($"elapsed = {elapsed} ms / lag = {lag} ms");
                    game.Input();

                    while (lag >= MS_PER_UPDATE)
                    {
                        game.Update(MS_PER_UPDATE);
                        lag -= MS_PER_UPDATE;
                    }


                    game.Render(lag / MS_PER_UPDATE);

                    //lastTime = current;
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