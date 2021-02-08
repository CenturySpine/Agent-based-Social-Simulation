using System;
using System.Diagnostics;
using SocialSimulation.Game;
using System.Threading;
using System.Threading.Tasks;

namespace SocialSimulation.GameLoop
{
    internal class StandardTimerGameLoop : IGameLoop
    {
        private Timer _moveTimer;
        private bool _running;
        private Stopwatch _sw;
        private double _lastUpdate;

        public void Start(IGame game)
        {
            _running = true;
            _sw = new Stopwatch();
            _sw.Reset();
            _sw.Start();
            _lastUpdate = _sw.Elapsed.TotalMilliseconds;
            //_moveTimer = new Timer(OnUpdateEntities, game, 0, (int)SimLoopData.DesiredElapsed);
            Task.Run(() =>
            {
                while (_running)
                {
                    InsideLoop(game);
                }
            });

        }

        private void OnUpdateEntities(object state)
        {
            InsideLoop(state);
        }

        private void InsideLoop(object state)
        {
            if (!_running) return;
            if (state is IGame game)
            {
                var _current = _sw.Elapsed.TotalMilliseconds;
                float elpased = (float)(_current - _lastUpdate);
                _lastUpdate = _current;

                game.Input();
                game.Update(elpased);
                game.Render(1);
                Console.WriteLine(elpased);
                if (elpased < SimLoopData.DesiredElapsed)
                {
                    var diff = TimeSpan.FromMilliseconds(SimLoopData.DesiredElapsed - elpased);
                    Console.WriteLine("sleep:" + diff.TotalMilliseconds + "ms");
                    Thread.Sleep(diff);
                }
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