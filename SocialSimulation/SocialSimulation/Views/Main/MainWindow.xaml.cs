using System.Windows;

namespace SocialSimulation.Views.Main
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}