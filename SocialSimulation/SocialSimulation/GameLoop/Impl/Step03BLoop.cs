using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialSimulation.Game;

namespace SocialSimulation.GameLoop.Impl
{
    class Step03BLoop : ICustomLoopBehavior
    {
        private bool _running;
        const int constantFPS = 60;
        const double dt = 1000 / constantFPS;
        double accumulatedTime = 0.0;


        public void Start(IGame game)
        {


            _running = true;
            Task.Run(() =>
            {

                var sw = Stopwatch.StartNew();
                sw.Start();
                while (_running)
                {

                    // acquire input
                    game.Input();

                    // get time elapsed since the last frame
                    accumulatedTime += GetDeltaTime(sw);

                    // now update the game logic based on the input and the elapsed time since the last frame
                    while (accumulatedTime >= dt)
                    {
                        game.Update((float)dt);
                        accumulatedTime -= dt;
                    }


                    // generate output
                    game.Render((float)(accumulatedTime / dt));

                    //sw.Start();
                }
            });
        }

        public void Stop(IGame game)
        {
            _running = false;
            game.Unload();
        }

        double GetDeltaTime(Stopwatch sw)
        {

            // Do something here

            //sw.Stop();

            return sw.ElapsedMilliseconds;
            // or sw.ElapsedTicks
        }
    }
}
