using SocialSimulation.Game;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocialSimulation.GameLoop
{
    internal class CustomGameLoop : IGameLoop
    {
        private readonly ICustomLoopBehavior _loopBehavior;

        public CustomGameLoop()
        {
            _loopBehavior = new Step01Loop();
            //_loopBehavior = new Step02Loop();
            //_loopBehavior = new Step03Loop();
        }

        public void Start(IGame game)
        {
            _loopBehavior.Start(game);
        }

        public void Stop(IGame game)
        {
            _loopBehavior.Stop(game);
        }

        private interface ICustomLoopBehavior
        {
            void Start(IGame game);

            void Stop(IGame game);
        }

        private class Step03Loop : ICustomLoopBehavior
        {
            private bool _running;
            private TimeSpan MS_PER_FRAME = new TimeSpan(0, 0, 0, 0, (int)SimLoopData.Elapsed);

            public void Start(IGame game)
            {
                TimeSpan previous = TimeSpan.FromTicks(DateTime.Now.Ticks);
                float lag = 0.0f;

                _running = true;
                Task.Run(() =>
                {
                    while (_running)
                    {
                        TimeSpan current = TimeSpan.FromTicks(DateTime.Now.Ticks);
                        TimeSpan elapsed = current - previous;
                        previous = current;

                        lag += elapsed.Milliseconds;

                        game.Input();

                        while (lag >= MS_PER_FRAME.Milliseconds)
                        {
                            game.Update(1);
                            lag -= MS_PER_FRAME.Milliseconds;
                        }
                       
                        
                        game.Render(elapsed.Milliseconds);

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
        //shitty, laggy
        private class Step02Loop : ICustomLoopBehavior
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

        //not bad, good visual feel, entity speed must be increased
        private class Step01Loop : ICustomLoopBehavior
        {
            private TimeSpan MS_PER_FRAME = new TimeSpan(0, 0, 0, 0, (int)SimLoopData.Elapsed);
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
}