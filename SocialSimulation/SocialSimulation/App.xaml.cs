using System.Linq;
using System.Reflection;
using System.Windows;
using SimpleInjector;

namespace SocialSimulation
{
    public partial class App : Application
    {
        private readonly Container _container = new Container();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Register();

            App.Current.MainWindow = _container.GetInstance<MainWindow>();
            App.Current.MainWindow.Show();
        }

        private void Register()
        {

            _container.Register<MainWindow>();
            _container.Register<MainViewModel>(Lifestyle.Singleton);
            _container.Register<GlobalSimulationParameters>(Lifestyle.Singleton);
            _container.Register<MovementService>(Lifestyle.Singleton);

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
