namespace SocialSimulation.Game
{
    public interface IGame
    {
        void Load();

        void Update(float elapsed);

        void Render(float elapsed);

        void Unload();

        void Input();
    }
}