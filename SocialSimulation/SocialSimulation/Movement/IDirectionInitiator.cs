using SocialSimulation.SimulationParameters;
using System.Numerics;

namespace SocialSimulation.Movement
{
    public interface IDirectionInitiator
    {
        Vector2 InitiateDirectionGoal(Entity.Entity entity, GlobalSimulationParameters parameters);
    }
}