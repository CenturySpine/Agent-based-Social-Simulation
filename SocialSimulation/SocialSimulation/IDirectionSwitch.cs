using System;

namespace SocialSimulation
{
    internal interface IDirectionSwitch
    {
        void Switch(Entity e, Random rnd);
    }
}