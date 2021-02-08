using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialSimulation.Game;

namespace SocialSimulation.GameLoop.Impl
{
    interface ICustomLoopBehavior
    {
        void Start(IGame game);

        void Stop(IGame game);
    }
}
