using SocialSimulation.Game;
using System.Diagnostics;
using SocialSimulation.GameLoop.Impl;

namespace SocialSimulation.GameLoop
{
    internal class CustomGameLoop : IGameLoop
    {
        private readonly ICustomLoopBehavior _loopBehavior;

        public CustomGameLoop()
        {
            _loopBehavior = new Step03Loop();
            //_loopBehavior = new Step02Loop();
            //_loopBehavior = new Step03Loop();
            //_loopBehavior = new Step03BLoop(); 
        }

        public void Start(IGame game)
        {
            _loopBehavior.Start(game);
        }

        public void Stop(IGame game)
        {
            _loopBehavior.Stop(game);
        }



    }

    //shitty, laggy

    //not bad, good visual feel, entity speed must be increased
}