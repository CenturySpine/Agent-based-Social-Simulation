using SocialSimulation.Game;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocialSimulation.GameLoop.Impl
{
    internal class AnotherLoop : ICustomLoopBehavior
    {
        private IGame _game;
        private bool _isRunning;

        public void Start(IGame game)
        {
            if (_isRunning) return;

            _game = game;
            int UPDATES_PER_SECOND = 25;
            int WAIT_TICKS = 1000 / UPDATES_PER_SECOND;
            int MAX_FRAMESKIP = 5;

            long next_update = DateTime.Now.Millisecond;

            /////// NEW CODE BEGIN
            int MAX_UPDATES_PER_SECOND = 60;
            int MIN_WAIT_TICKS = 1000 / MAX_UPDATES_PER_SECOND;

            long last_update = DateTime.Now.Millisecond;

            int frames_skipped;
            float interpolation;

            Task.Run(() =>
            {
                _isRunning = true;
                // Start the loop:
                while (_isRunning)
                {
                    /////// NEW CODE BEGIN
                    // Delay if needed
                    while (DateTime.Now.Millisecond < last_update + MIN_WAIT_TICKS)
                    {
                        Thread.Sleep(0); // I don't know C# so this is a guess, but there will be some equivalent function somewhere
                    }
                    last_update = DateTime.Now.Millisecond;
                    /////// NEW CODE END

                    // Update game:
                    frames_skipped = 0;
                    while (DateTime.Now.Millisecond > next_update
                           && frames_skipped < MAX_FRAMESKIP)
                    {
                        _game.Input();
                        _game.Update(1);
                        // Schedule next update:
                        next_update += WAIT_TICKS;
                        frames_skipped++;
                    }

                    // Calculate interpolation for smooth animation between states:
                    interpolation = ((float)(DateTime.Now.Millisecond + WAIT_TICKS - next_update)) / ((float)WAIT_TICKS);

                    // Render-events:
                    _game.Render(interpolation);
                }
            });
        }

        public void Stop(IGame game)
        {
            _isRunning = false;
        }
    }
}