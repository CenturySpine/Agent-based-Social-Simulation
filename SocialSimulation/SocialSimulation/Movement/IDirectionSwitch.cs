using System;

namespace SocialSimulation.Movement
{
    internal interface IDirectionSwitch
    {
        void Switch(Entity.Entity e, Random rnd);
    }
}