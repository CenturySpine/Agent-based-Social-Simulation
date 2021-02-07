using SimpleInjector;
using SocialSimulation.Collisions;
using SocialSimulation.Entity;
using SocialSimulation.Environment;
using SocialSimulation.Game;
using SocialSimulation.GameLoop;
using SocialSimulation.Interactions;
using SocialSimulation.Movement;
using SocialSimulation.SimulationParameters;
using SocialSimulation.Views.Main;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace SocialSimulation
{
    public partial class App : Application
    {
        private readonly Container _container = new Container();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Register();

            _container.GetInstance<Logger>().RegisterListener(ConsoleLog);

            App.Current.MainWindow = _container.GetInstance<MainWindow>();
            App.Current.MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _container.GetInstance<MainViewModel>().Unload();
        }

        private void ConsoleLog(string obj)
        {
            Console.WriteLine(obj);
        }

        private void Register()
        {
            _container.Register<MainWindow>();
            _container.Register<MainViewModel>(Lifestyle.Singleton);
            _container.Register<GlobalSimulationParameters>(Lifestyle.Singleton);
            _container.Register<MovementService>(Lifestyle.Singleton);
            _container.Register<Logger>(Lifestyle.Singleton);
            _container.Register<CollisionService>(Lifestyle.Singleton);
            _container.Register<InteractionService>(Lifestyle.Singleton);
            _container.Register<EnvironmentService>(Lifestyle.Singleton);
            _container.Register<IGame, MyGame>(Lifestyle.Singleton);

            //_container.Register<IGameLoop, StandardTimerGameLoop>(Lifestyle.Singleton);
            //_container.Register<IGameLoop, CompositionTargetLoop>(Lifestyle.Singleton);
            _container.Register<IGameLoop, CustomGameLoop>(Lifestyle.Singleton);
            


            _container.Register<IGameSurface, CanvasGameSurface>(Lifestyle.Singleton);

            var behaviors = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => !t.IsInterface && typeof(IEntityBehavior).IsAssignableFrom(t))
                .ToList();

            foreach (var behavior in behaviors)
            {
                _container.Register(behavior);
            }
        }
    }
}