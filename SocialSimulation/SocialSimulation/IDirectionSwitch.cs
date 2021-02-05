using System;

namespace SocialSimulation
{
    internal interface IDirectionSwitch
    {
        void Switch(Entity.Entity e, Random rnd);
    }
}