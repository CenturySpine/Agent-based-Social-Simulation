using System.Numerics;

namespace SocialSimulation
{
    public interface IDirectionInitiator
    {
        Vector2 InitiateDirectionGoal(Entity.Entity entity, GlobalSimulationParameters parameters);
    }
}