using System.Windows;
using SimpleInjector;

namespace SocialSimulation
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Container _container = new Container();

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
            _container.Register<MainViewModel>();
        }
    }
}
