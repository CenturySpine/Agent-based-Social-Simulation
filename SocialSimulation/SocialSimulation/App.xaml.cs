using System;
using SimpleInjector;
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