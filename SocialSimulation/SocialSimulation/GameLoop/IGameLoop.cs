using SocialSimulation.Game;

namespace SocialSimulation.GameLoop
{
    public interface IGameLoop
    {
        void Start(IGame game);

        void Stop(IGame game);
    }
}